set nocount on;
set xact_abort on;

if object_id('dbo.authors', 'u') is null
begin
    create table dbo.authors (
        authorId     int identity(1,1) not null constraint pk_authors primary key,
        firstName    nvarchar(50)  not null,
        lastName     nvarchar(100) not null,
        dateOfBirth  date not null,
        deathDate    date null,
        constraint ck_authors_dates check (deathDate is null or deathDate >= dateOfBirth)
    );
end;

if object_id('dbo.publishers', 'u') is null
begin
    create table dbo.publishers (
        publisherId   int identity(1,1) not null constraint pk_publishers primary key,
        publisherName nvarchar(120) not null,
        country       nvarchar(60) null
    );
end;

if object_id('dbo.categories', 'u') is null
begin
    create table dbo.categories (
        categoryId   int identity(1,1) not null constraint pk_categories primary key,
        categoryName nvarchar(80) not null constraint uq_categories_name unique
    );
end;

if object_id('dbo.books', 'u') is null
begin
    create table dbo.books (
        isbn         char(13) not null constraint pk_books primary key,
        title        nvarchar(500) not null,
        [language]   nvarchar(15) not null,
        price        decimal(10,2) not null constraint ck_books_price check (price >= 0),
        publishDate  date not null,
        pages        int null constraint ck_books_pages check (pages is null or pages > 0),
        [format]     nvarchar(25) null, -- paperback, hardcover, ebook
        publisherId  int null,
        constraint ck_books_isbn check (isbn not like '%[^0-9]%'),
        constraint fk_books_publishers foreign key (publisherId) references dbo.publishers(publisherId)
    );

    create index ix_books_publisherId on dbo.books(publisherId);
end;


if object_id('dbo.bookAuthors', 'u') is null
begin
    create table dbo.bookAuthors (
        isbn     char(13) not null,
        authorId int not null,
        role     nvarchar(30) null,
        constraint pk_bookAuthors primary key (isbn, authorId),
        constraint fk_bookAuthors_books foreign key (isbn) references dbo.books(isbn),
        constraint fk_bookAuthors_authors foreign key (authorId) references dbo.authors(authorId)
    );

    create index ix_bookAuthors_authorId on dbo.bookAuthors(authorId);
end;

-- books <-> categories
if object_id('dbo.bookCategories', 'u') is null
begin
    create table dbo.bookCategories (
        isbn       char(13) not null,
        categoryId int not null,
        constraint pk_bookCategories primary key (isbn, categoryId),
        constraint fk_bookCategories_books foreign key (isbn) references dbo.books(isbn),
        constraint fk_bookCategories_categories foreign key (categoryId) references dbo.categories(categoryId)
    );
end;

if object_id('dbo.stores', 'u') is null
begin
    create table dbo.stores (
        storeId   int identity(1,1) not null constraint pk_stores primary key,
        storeName nvarchar(60) not null,
        [address] nvarchar(120) not null
    );
end;

if object_id('dbo.bookBalance', 'u') is null
begin
    create table dbo.bookBalance (
        storeId       int not null,
        isbn          char(13) not null,
        amountInStock int not null constraint ck_bookBalance_stock check (amountInStock >= 0),
        constraint pk_bookBalance primary key (storeId, isbn),
        constraint fk_bookBalance_store foreign key (storeId) references dbo.stores(storeId),
        constraint fk_bookBalance_book  foreign key (isbn) references dbo.books(isbn)
    );

    create index ix_bookBalance_isbn on dbo.bookBalance(isbn);
end;

-- =========================

if object_id('dbo.customers', 'u') is null
begin
    create table dbo.customers (
        customerId   int identity(1,1) not null constraint pk_customers primary key,
        firstName    nvarchar(50) not null,
        lastName     nvarchar(80) not null,
        email        nvarchar(150) not null,
        phone        nvarchar(20) not null,
        registeredAt datetime2(0) not null constraint df_customers_registeredAt default sysdatetime(),
        constraint uq_customers_email unique (email)
    );
end;

if object_id('dbo.orders', 'u') is null
begin
    create table dbo.orders (
        orderId     int identity(1,1) not null constraint pk_orders primary key,
        customerId  int not null,
        storeId     int not null,
        orderDate   datetime2(0) not null constraint df_orders_orderDate default sysdatetime(),
        totalAmount decimal(12,2) not null constraint ck_orders_total check (totalAmount >= 0),
        constraint fk_orders_customer foreign key (customerId) references dbo.customers(customerId),
        constraint fk_orders_store    foreign key (storeId) references dbo.stores(storeId)
    );

    create index ix_orders_customerId on dbo.orders(customerId);
    create index ix_orders_storeId on dbo.orders(storeId);
end;

if object_id('dbo.orderDetails', 'u') is null
begin
    create table dbo.orderDetails (
        orderId   int not null,
        isbn      char(13) not null,
        quantity  int not null constraint ck_orderDetails_qty check (quantity > 0),
        bookPrice decimal(10,2) not null constraint ck_orderDetails_price check (bookPrice >= 0),
        constraint pk_orderDetails primary key (orderId, isbn),
        constraint fk_orderDetails_order foreign key (orderId) references dbo.orders(orderId),
        constraint fk_orderDetails_book  foreign key (isbn) references dbo.books(isbn)
    );

    create index ix_orderDetails_isbn on dbo.orderDetails(isbn);
end;

-- =========================

if not exists (select 1 from dbo.publishers)
begin
    insert into dbo.publishers (publisherName, country) values
    (N'penguin classics', N'uk'),
    (N'oxford world''s classics', N'uk'),
    (N'vintage', N'uk');
end;

if not exists (select 1 from dbo.categories)
begin
    insert into dbo.categories (categoryName) values
    (N'philosophy'),
    (N'fiction'),
    (N'classics'),
    (N'politics'),
    (N'psychology');
end;

if not exists (select 1 from dbo.authors)
begin
    insert into dbo.authors(firstName, lastName, dateOfBirth, deathDate) values
    (N'friedrich',  N'nietzsche',     '1844-10-15', '1900-08-25'),
    (N'fjodor',     N'dostoevsky',    '1821-11-11', '1881-02-09'),
    (N'arthur',     N'schopenhauer',  '1788-02-22', '1860-09-21'),
    (N'albert',     N'camus',         '1913-11-07', '1960-01-04'),
    (N'jean-paul',  N'sartre',        '1905-06-21', '1980-04-15'),
    (N'søren',      N'kierkegaard',   '1813-05-05', '1855-11-11'),
    (N'karl',       N'marx',          '1818-05-05', '1883-03-14'),
    (N'friedrich',  N'engels',        '1820-11-28', '1895-08-05');
end;

if not exists (select 1 from dbo.books)
begin
    declare @pub1 int = (select top 1 publisherId from dbo.publishers where publisherName = N'penguin classics');
    declare @pub2 int = (select top 1 publisherId from dbo.publishers where publisherName = N'oxford world''s classics');
    declare @pub3 int = (select top 1 publisherId from dbo.publishers where publisherName = N'vintage');

    insert into dbo.books(isbn, title, [language], price, publishDate, pages, [format], publisherId) values
    ('9780140441185', N'thus spoke zarathustra',               N'english', 169.00, '2005-11-29', 480, N'paperback', @pub1),
    ('9780140442038', N'beyond good and evil',                 N'english', 159.00, '2003-04-29', 352, N'paperback', @pub1),
    ('9780140449136', N'crime and punishment',                 N'english', 229.00, '2003-02-27', 720, N'paperback', @pub1),
    ('9780140449242', N'the brothers karamazov',               N'english', 219.00, '2003-02-27', 1056, N'paperback', @pub1),
    ('9780486434103', N'the world as will and representation', N'english', 279.00, '2005-06-10', 544, N'paperback', @pub2),
    ('9780141186542', N'the myth of sisyphus',                 N'french',  159.00, '2005-11-03', 224, N'paperback', @pub1),
    ('9780140444490', N'nausea',                               N'french',  189.00, '2000-11-30', 256, N'paperback', @pub1),
    ('9780140445718', N'fear and trembling',                   N'danish',  159.00, '2003-09-25', 144, N'paperback', @pub1),
    ('9780141018934', N'the stranger',                         N'french',  179.00, '2000-01-01', 128, N'paperback', @pub3),
    ('9780140447576', N'the communist manifesto',              N'english', 149.00, '2002-01-01', 144, N'paperback', @pub1);
end;

-- link books <-> authors (vg: at least one book with multiple authors)
if not exists (select 1 from dbo.bookAuthors)
begin
    declare @nietzsche int = (select authorId from dbo.authors where firstName = N'friedrich' and lastName = N'nietzsche');
    declare @dosto     int = (select authorId from dbo.authors where firstName = N'fjodor' and lastName = N'dostoevsky');
    declare @schop     int = (select authorId from dbo.authors where firstName = N'arthur' and lastName = N'schopenhauer');
    declare @camus     int = (select authorId from dbo.authors where firstName = N'albert' and lastName = N'camus');
    declare @sartre    int = (select authorId from dbo.authors where firstName = N'jean-paul' and lastName = N'sartre');
    declare @kierk     int = (select authorId from dbo.authors where firstName = N'søren' and lastName = N'kierkegaard');
    declare @marx      int = (select authorId from dbo.authors where firstName = N'karl' and lastName = N'marx');
    declare @engels    int = (select authorId from dbo.authors where firstName = N'friedrich' and lastName = N'engels');

    insert into dbo.bookAuthors(isbn, authorId, role) values
    ('9780140441185', @nietzsche, N'author'),
    ('9780140442038', @nietzsche, N'author'),
    ('9780140449136', @dosto,     N'author'),
    ('9780140449242', @dosto,     N'author'),
    ('9780486434103', @schop,     N'author'),
    ('9780141186542', @camus,     N'author'),
    ('9780140444490', @sartre,    N'author'),
    ('9780140445718', @kierk,     N'author'),
    ('9780141018934', @camus,     N'author'),
    ('9780140447576', @marx,      N'author'),
    ('9780140447576', @engels,    N'author');
end;

-- link books <-> categories
if not exists (select 1 from dbo.bookCategories)
begin
    declare @catPhil int = (select categoryId from dbo.categories where categoryName = N'philosophy');
    declare @catFic  int = (select categoryId from dbo.categories where categoryName = N'fiction');
    declare @catCla  int = (select categoryId from dbo.categories where categoryName = N'classics');
    declare @catPol  int = (select categoryId from dbo.categories where categoryName = N'politics');
    declare @catPsy  int = (select categoryId from dbo.categories where categoryName = N'psychology');

    insert into dbo.bookCategories(isbn, categoryId) values
    ('9780140441185', @catPhil),
    ('9780140441185', @catCla),
    ('9780140442038', @catPhil),
    ('9780140449136', @catFic),
    ('9780140449136', @catCla),
    ('9780140449242', @catFic),
    ('9780486434103', @catPhil),
    ('9780141186542', @catPhil),
    ('9780140444490', @catFic),
    ('9780140445718', @catPhil),
    ('9780140445718', @catPsy),
    ('9780141018934', @catFic),
    ('9780140447576', @catPol);
end;

-- stores
if not exists (select 1 from dbo.stores)
begin
    insert into dbo.stores (storeName, [address]) values
    (N'Ord & Bläck',  N'Sveavägen 23, Stockholm'),
    (N'Sidvändaren',  N'Kungsgatan 18, Göteborg'),
    (N'Bokhörnan',    N'Södra Förstadsgatan 12, Malmö');
end;

-- bookBalance: saldo för varje butik för alla 10 titlar (3 * 10 = 30 rows)
if not exists (select 1 from dbo.bookBalance)
begin
    insert into dbo.bookBalance (storeId, isbn, amountInStock) values
    -- store 1
    (1,'9780140441185', 12),(1,'9780140442038', 7),(1,'9780140449136', 5),(1,'9780140449242', 4),(1,'9780486434103', 6),
    (1,'9780141186542', 9),(1,'9780140444490', 8),(1,'9780140445718', 10),(1,'9780141018934', 11),(1,'9780140447576', 3),
    -- store 2
    (2,'9780140441185', 6),(2,'9780140442038', 9),(2,'9780140449136', 2),(2,'9780140449242', 7),(2,'9780486434103', 4),
    (2,'9780141186542', 5),(2,'9780140444490', 6),(2,'9780140445718', 3),(2,'9780141018934', 8),(2,'9780140447576', 5),
    -- store 3
    (3,'9780140441185', 10),(3,'9780140442038', 4),(3,'9780140449136', 6),(3,'9780140449242', 3),(3,'9780486434103', 2),
    (3,'9780141186542', 7),(3,'9780140444490', 5),(3,'9780140445718', 6),(3,'9780141018934', 4),(3,'9780140447576', 9);
end;

-- customers
if not exists (select 1 from dbo.customers)
begin
    insert into dbo.customers (firstName, lastName, email, phone) values
    (N'Anna',  N'Andersson',  N'anna.andersson@mail.se',  N'0701234567'),
    (N'Erik',  N'Johansson',  N'erik.johansson@mail.se',  N'0729876543'),
    (N'Sara',  N'Nilsson',    N'sara.nilsson@mail.se',    N'0731112233'),
    (N'Oscar', N'Lindberg',   N'oscar.lindberg@mail.se',  N'0765554433'),
    (N'Maja',  N'Hedlund',    N'maja.hedlund@mail.se',    N'0709988776');
end;

-- orders + orderDetails (enkelt: totals matchar exakt)
if not exists (select 1 from dbo.orders)
begin
    insert into dbo.orders (customerId, storeId, totalAmount) values
    (1, 1, 169.00),
    (2, 2, 229.00),
    (3, 3, 318.00),
    (4, 1, 159.00),
    (5, 2, 298.00),
    (1, 3, 149.00);
end;

if not exists (select 1 from dbo.orderDetails)
begin
    insert into dbo.orderDetails (orderId, isbn, quantity, bookPrice) values
    (1, '9780140441185', 1, 169.00),
    (2, '9780140449136', 1, 229.00),
    (3, '9780140442038', 1, 159.00),
    (3, '9780141186542', 1, 159.00), -- 159 + 159 = 318
    (4, '9780140442038', 1, 159.00),
    (5, '9780140447576', 2, 149.00), -- 2*149 = 298
    (6, '9780140447576', 1, 149.00);
end;

-- =========================

if object_id('dbo.TitlarPerForfattare', 'v') is not null
    drop view dbo.TitlarPerForfattare;
go

create view dbo.TitlarPerForfattare as
select
    concat(a.firstName, N' ', a.lastName) as [Namn],
    concat(
        datediff(year, a.dateOfBirth, coalesce(a.deathDate, cast(getdate() as date)))
        - case
            when dateadd(year, datediff(year, a.dateOfBirth, coalesce(a.deathDate, cast(getdate() as date))), a.dateOfBirth)
                 > coalesce(a.deathDate, cast(getdate() as date))
            then 1 else 0 end
    , N' år') as [Ålder],
    concat(count(distinct b.isbn), N' st') as [Titlar],
    concat(format(isnull(sum(cast(bb.amountInStock as bigint) * b.price), 0), 'N0'), N' kr') as [Lagervärde]
from dbo.authors a
join dbo.bookAuthors ba on ba.authorId = a.authorId
join dbo.books b on b.isbn = ba.isbn
left join dbo.bookBalance bb on bb.isbn = b.isbn
group by a.firstName, a.lastName, a.dateOfBirth, a.deathDate;
go

-- demo
select top 1 * from dbo.TitlarPerForfattare;

-- =========================

if object_id('dbo.ButikFörsäljning', 'v') is not null
    drop view dbo.ButikFörsäljning;
go

create view dbo.ButikFörsäljning as
select
    s.storeId,
    s.storeName as [butik],
    count(o.orderId) as [antalOrdrar],
    count(distinct o.customerId) as [unikaKunder],
    sum(o.totalAmount) as [omsättning],
    avg(o.totalAmount) as [snittOrder]
from dbo.stores s
left join dbo.orders o on o.storeId = s.storeId
group by s.storeId, s.storeName;
go

-- demo
select * from dbo.ButikFörsäljning order by omsättning desc;

-- =========================

if object_id('dbo.FlyttaBok', 'p') is not null
    drop procedure dbo.FlyttaBok;
go

create procedure dbo.FlyttaBok
    @fromStoreId int,
    @toStoreId   int,
    @isbn        char(13),
    @qty         int = 1
as
begin
    set nocount on;
    set xact_abort on;

    if @qty is null or @qty <= 0
        throw 50001, 'qty must be > 0', 1;

    if @fromStoreId = @toStoreId
        throw 50002, 'from and to store cannot be the same', 1;

    begin tran;

    if not exists (select 1 from dbo.stores where storeId = @fromStoreId)
        throw 50003, 'from store does not exist', 1;

    if not exists (select 1 from dbo.stores where storeId = @toStoreId)
        throw 50004, 'to store does not exist', 1;

    if not exists (select 1 from dbo.books where isbn = @isbn)
        throw 50005, 'isbn does not exist', 1;

    declare @fromStock int;

    select @fromStock = amountInStock
    from dbo.bookBalance with (updlock, holdlock)
    where storeId = @fromStoreId and isbn = @isbn;

    if @fromStock is null or @fromStock < @qty
        throw 50006, 'not enough stock in from store', 1;

    if not exists (
        select 1
        from dbo.bookBalance with (updlock, holdlock)
        where storeId = @toStoreId and isbn = @isbn
    )
    begin
        insert into dbo.bookBalance(storeId, isbn, amountInStock)
        values (@toStoreId, @isbn, 0);
    end;

    update dbo.bookBalance
    set amountInStock = amountInStock - @qty
    where storeId = @fromStoreId and isbn = @isbn;

    update dbo.bookBalance
    set amountInStock = amountInStock + @qty
    where storeId = @toStoreId and isbn = @isbn;

    commit;
end;
go

-- demo (flytta 2 ex från butik 1 till butik 2)
-- exec dbo.FlyttaBok @fromStoreId = 1, @toStoreId = 2, @isbn = '9780140441185', @qty = 2;

-- =========
-- teardown
-- ========
/*
drop procedure if exists dbo.FlyttaBok;

drop view if exists dbo.ButikFörsäljning;
drop view if exists dbo.TitlarPerForfattare;

drop table if exists dbo.orderDetails;
drop table if exists dbo.orders;
drop table if exists dbo.customers;

drop table if exists dbo.bookBalance;
drop table if exists dbo.stores;

drop table if exists dbo.bookCategories;
drop table if exists dbo.bookAuthors;

drop table if exists dbo.books;
drop table if exists dbo.categories;
drop table if exists dbo.publishers;
drop table if exists dbo.authors;
*/
