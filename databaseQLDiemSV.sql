USE master;
GO

-- 1. TẠO DATABASE (Nếu chưa có)
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'QuanLyDiemSV')
BEGIN
    CREATE DATABASE QuanLyDiemSV;
END
GO

USE QuanLyDiemSV;
GO

-- =============================================================
-- XÓA BẢNG CŨ (Theo thứ tự để tránh lỗi khóa ngoại khi chạy lại)
-- =============================================================
IF OBJECT_ID('KetQua', 'U') IS NOT NULL DROP TABLE KetQua;
IF OBJECT_ID('LopHocPhan', 'U') IS NOT NULL DROP TABLE LopHocPhan;
IF OBJECT_ID('SinhVien', 'U') IS NOT NULL DROP TABLE SinhVien;
IF OBJECT_ID('GiangVien', 'U') IS NOT NULL DROP TABLE GiangVien;
IF OBJECT_ID('MonHoc', 'U') IS NOT NULL DROP TABLE MonHoc;
IF OBJECT_ID('Lop', 'U') IS NOT NULL DROP TABLE Lop;
IF OBJECT_ID('Khoa', 'U') IS NOT NULL DROP TABLE Khoa;
IF OBJECT_ID('TaiKhoan', 'U') IS NOT NULL DROP TABLE TaiKhoan;
GO

-- =============================================================
-- TẠO CÁC BẢNG (Chuẩn theo DTO C#)
-- =============================================================

-- 1. Bảng TAI KHOAN (DTO_TaiKhoan.cs)
CREATE TABLE TaiKhoan (
    TenDangNhap VARCHAR(50) PRIMARY KEY,
    MatKhau VARCHAR(100) NOT NULL,
    Quyen VARCHAR(20) NOT NULL -- Admin, GiangVien, SinhVien
);

-- 2. Bảng KHOA (Bảng phụ trợ để liên kết dữ liệu)
CREATE TABLE Khoa (
    MaKhoa VARCHAR(20) PRIMARY KEY,
    TenKhoa NVARCHAR(100) NOT NULL
);

-- 3. Bảng LOP (Bảng phụ trợ để liên kết dữ liệu)
CREATE TABLE Lop (
    MaLop VARCHAR(20) PRIMARY KEY,
    TenLop NVARCHAR(100) NOT NULL,
    MaKhoa VARCHAR(20) FOREIGN KEY REFERENCES Khoa(MaKhoa)
);

-- 4. Bảng MON HOC (DTO_MonHoc.cs)
-- Quan trọng: Tên cột phải là TenMH để khớp với property trong DTO
CREATE TABLE MonHoc (
    MaMH VARCHAR(20) PRIMARY KEY, 
    TenMH NVARCHAR(100) NOT NULL, 
    SoTinChi INT NOT NULL
);

-- 5. Bảng GIANG VIEN (DTO_GiangVien.cs)
-- Quan trọng: Tên cột phải là HoTen để khớp với property trong DTO
CREATE TABLE GiangVien (
    MaGV VARCHAR(20) PRIMARY KEY,
    HoTen NVARCHAR(100) NOT NULL, 
    Email VARCHAR(100),
    SDT VARCHAR(15),
    MaKhoa VARCHAR(20) FOREIGN KEY REFERENCES Khoa(MaKhoa)
);

-- 6. Bảng SINH VIEN (DTO_SinhVien.cs)
CREATE TABLE SinhVien (
    MSSV VARCHAR(20) PRIMARY KEY,
    HoTen NVARCHAR(100) NOT NULL,
    NgaySinh DATE,
    GioiTinh NVARCHAR(10),
    Email VARCHAR(100),
    SDT VARCHAR(15),
    DiaChi NVARCHAR(200),
    MaLop VARCHAR(20) FOREIGN KEY REFERENCES Lop(MaLop)
);

-- 7. Bảng LOP HOC PHAN (DTO_LopHocPhan.cs)
CREATE TABLE LopHocPhan (
    MaLHP VARCHAR(20) PRIMARY KEY,
    
    -- Lưu ý: Trong DTO_LopHocPhan thuộc tính tên là "MaMon", nên ở đây đặt tên cột là MaMon cho dễ map
    MaMon VARCHAR(20) NOT NULL FOREIGN KEY REFERENCES MonHoc(MaMH),
    
    MaGV VARCHAR(20) NOT NULL FOREIGN KEY REFERENCES GiangVien(MaGV),
    
    HocKy VARCHAR(10),
    NamHoc VARCHAR(20),
    TyLeQT FLOAT,
    TyLeCK FLOAT,
    
    -- Các trường lịch học (Bạn mới bổ sung)
    Thu INT,             -- 2, 3, 4, 5...
    TietBD INT,          -- Tiết bắt đầu
    SoTiet INT,          -- Số tiết
    Phong VARCHAR(20)    -- Phòng học
);

-- 8. Bảng KET QUA (DTO_KetQua.cs)
CREATE TABLE KetQua (
    MaLHP VARCHAR(20),
    MSSV VARCHAR(20),
    DiemGK FLOAT DEFAULT NULL,
    DiemCK FLOAT DEFAULT NULL,
    DiemTK FLOAT DEFAULT NULL,
    DiemChu VARCHAR(5) DEFAULT NULL,
    
    PRIMARY KEY (MaLHP, MSSV),
    FOREIGN KEY (MaLHP) REFERENCES LopHocPhan(MaLHP),
    FOREIGN KEY (MSSV) REFERENCES SinhVien(MSSV)
);
GO

-- =============================================================
-- 1. XÓA SẠCH DỮ LIỆU CŨ
-- =============================================================
ALTER TABLE KetQua NOCHECK CONSTRAINT ALL;
ALTER TABLE LopHocPhan NOCHECK CONSTRAINT ALL;
ALTER TABLE SinhVien NOCHECK CONSTRAINT ALL;
ALTER TABLE GiangVien NOCHECK CONSTRAINT ALL;
ALTER TABLE Lop NOCHECK CONSTRAINT ALL;
ALTER TABLE MonHoc NOCHECK CONSTRAINT ALL;
ALTER TABLE Khoa NOCHECK CONSTRAINT ALL;
ALTER TABLE TaiKhoan NOCHECK CONSTRAINT ALL;

DELETE FROM KetQua;
DELETE FROM LopHocPhan;
DELETE FROM SinhVien;
DELETE FROM GiangVien;
DELETE FROM MonHoc;
DELETE FROM Lop;
DELETE FROM Khoa;
DELETE FROM TaiKhoan;

-- =============================================================
-- 2. TẠO DỮ LIỆU NỀN (CHỈ NGÀNH CNTT)
-- =============================================================

-- A. Khoa (Duy nhất 1 khoa)
INSERT INTO Khoa VALUES ('CNTT', N'Công nghệ Thông tin');

-- B. Lớp (Tạo 10 lớp chuyên ngành thực tế)
INSERT INTO Lop VALUES 
('21CN1', N'Kỹ thuật phần mềm K21-01', 'CNTT'),
('21CN2', N'Kỹ thuật phần mềm K21-02', 'CNTT'),
('21HT1', N'Hệ thống thông tin K21', 'CNTT'),
('21AT1', N'An toàn thông tin K21', 'CNTT'),
('21MT1', N'Mạng máy tính & Viễn thông K21', 'CNTT'),
('22CN1', N'Kỹ thuật phần mềm K22-01', 'CNTT'),
('22CN2', N'Kỹ thuật phần mềm K22-02', 'CNTT'),
('22DL1', N'Khoa học dữ liệu & AI K22', 'CNTT'),
('22TM1', N'Thương mại điện tử K22', 'CNTT'),
('22IOT', N'Internet vạn vật (IoT) K22', 'CNTT');

-- C. Môn học (50 Môn chuyên ngành CNTT chuẩn)
INSERT INTO MonHoc (MaMH, TenMH, SoTinChi) VALUES 
-- Nhóm Cơ sở ngành
('NMLT', N'Nhập môn lập trình', 4),
('KTLT', N'Kỹ thuật lập trình', 4),
('CTDL', N'Cấu trúc dữ liệu và giải thuật', 3),
('CSDL', N'Cơ sở dữ liệu', 3),
('MMT', N'Mạng máy tính cơ bản', 3),
('KTMT', N'Kiến trúc máy tính và Hợp ngữ', 3),
('HDH', N'Hệ điều hành', 3),
('TRR', N'Toán rời rạc', 3),
('DSTT', N'Đại số tuyến tính', 3),
('XSTK', N'Xác suất thống kê ứng dụng', 3),

-- Nhóm Lập trình & Công nghệ phần mềm
('OOP', N'Lập trình hướng đối tượng (Java)', 4),
('WIN', N'Lập trình trực quan (C# .NET)', 4),
('WEB1', N'Lập trình Web 1 (HTML/CSS/JS)', 3),
('WEB2', N'Lập trình Web 2 (PHP/Laravel)', 4),
('WEB3', N'Lập trình Web Nâng cao (React/Node)', 4),
('LTDD1', N'Lập trình thiết bị di động (Android)', 4),
('LTDD2', N'Lập trình di động đa nền tảng (Flutter)', 4),
('PYTHON', N'Lập trình Python', 3),
('KTPM', N'Công nghệ phần mềm', 3),
('QLDA', N'Quản lý dự án phần mềm', 3),
('TEST', N'Kiểm thử phần mềm', 3),
('TKGD', N'Thiết kế giao diện người dùng (UI/UX)', 3),

-- Nhóm Dữ liệu & AI
('HQT', N'Hệ quản trị cơ sở dữ liệu (SQL Server)', 4),
('DM', N'Khai phá dữ liệu (Data Mining)', 3),
('BIGDATA', N'Dữ liệu lớn (Big Data)', 3),
('TTNT', N'Trí tuệ nhân tạo (AI)', 3),
('ML', N'Học máy (Machine Learning)', 3),
('DL', N'Học sâu (Deep Learning)', 3),
('NLP', N'Xử lý ngôn ngữ tự nhiên', 3),
('CV', N'Thị giác máy tính', 3),

-- Nhóm Mạng & An toàn
('QTM', N'Quản trị mạng Linux', 3),
('ATTT', N'An toàn thông tin', 3),
('MMH', N'Mật mã học', 3),
('ANM', N'An ninh mạng', 3),
('DDT', N'Điện toán đám mây (Cloud Computing)', 3),
('IOT', N'Lập trình IoT cơ bản', 3),
('BLOCK', N'Công nghệ Blockchain', 3),

-- Nhóm Kỹ năng & Đồ án
('TAVN', N'Tiếng Anh chuyên ngành CNTT', 2),
('KNM', N'Kỹ năng mềm cho dân IT', 2),
('DAMH', N'Đồ án môn học', 2),
('TTSN', N'Thực tập sản xuất', 2),
('KLTN', N'Khóa luận tốt nghiệp', 10);

-- =============================================================
-- 3. HÀM TỰ ĐỘNG SINH TÊN NGƯỜI VIỆT (Giữ nguyên logic cũ)
-- =============================================================
CREATE TABLE #Ho (Ho NVARCHAR(20));
CREATE TABLE #Dem (Dem NVARCHAR(20));
CREATE TABLE #Ten (Ten NVARCHAR(20));

INSERT INTO #Ho VALUES (N'Nguyễn'), (N'Trần'), (N'Lê'), (N'Phạm'), (N'Hoàng'), (N'Huỳnh'), (N'Phan'), (N'Vũ'), (N'Võ'), (N'Đặng'), (N'Bùi'), (N'Đỗ'), (N'Hồ'), (N'Ngô'), (N'Dương'), (N'Lý');
INSERT INTO #Dem VALUES (N'Văn'), (N'Thị'), (N'Minh'), (N'Hữu'), (N'Thanh'), (N'Đức'), (N'Hoàng'), (N'Ngọc'), (N'Quang'), (N'Xuân'), (N'Hải'), (N'Tuấn'), (N'Gia'), (N'Thảo'), (N'Phương'), (N'Công'), (N'Quốc'), (N'Thành');
INSERT INTO #Ten VALUES (N'Hùng'), (N'Dũng'), (N'Tuấn'), (N'Minh'), (N'Tâm'), (N'Thảo'), (N'Trang'), (N'Huyền'), (N'Nam'), (N'Bắc'), (N'Đông'), (N'Anh'), (N'Hà'), (N'Hương'), (N'Lan'), (N'Phúc'), (N'Lộc'), (N'Thọ'), (N'Toàn'), (N'Thắng'), (N'Lợi'), (N'Quân'), (N'Kiệt'), (N'Vy'), (N'Sơn'), (N'Duy'), (N'Khánh');

CREATE TABLE #DiaChi (DC NVARCHAR(50));
INSERT INTO #DiaChi VALUES (N'Hà Nội'), (N'TP. Hồ Chí Minh'), (N'Đà Nẵng'), (N'Hải Phòng'), (N'Cần Thơ'), (N'Huế'), (N'Nghệ An'), (N'Thanh Hóa'), (N'Quảng Ninh'), (N'Nam Định'), (N'Thái Bình'), (N'Hải Dương'), (N'Bắc Ninh'), (N'Đồng Nai'), (N'Bình Dương');

-- =============================================================
-- 4. SINH DỮ LIỆU GIẢNG VIÊN CNTT (50 Người)
-- =============================================================
DECLARE @i INT = 1;
DECLARE @MaGV VARCHAR(20);
DECLARE @HoTen NVARCHAR(50);
DECLARE @SDT VARCHAR(15);

WHILE @i <= 50
BEGIN
    SET @MaGV = 'GV' + RIGHT('000' + CAST(@i AS VARCHAR(3)), 3);
    
    SELECT TOP 1 @HoTen = H.Ho + ' ' + D.Dem + ' ' + T.Ten 
    FROM #Ho H CROSS JOIN #Dem D CROSS JOIN #Ten T ORDER BY NEWID();
    
    SET @SDT = '09' + CAST(CAST(RAND()*100000000 AS INT) AS VARCHAR(10));

    -- Tạo tài khoản
    INSERT INTO TaiKhoan VALUES (@MaGV, '123', 'GiangVien');
    
    -- Tạo giảng viên (Tất cả đều thuộc khoa CNTT)
    INSERT INTO GiangVien VALUES (@MaGV, @HoTen, LOWER(@MaGV) + '@fit.edu.vn', @SDT, 'CNTT');

    SET @i = @i + 1;
END

IF NOT EXISTS (SELECT * FROM TaiKhoan WHERE TenDangNhap = 'admin')
BEGIN
    INSERT INTO TaiKhoan VALUES ('admin', '123', 'Admin');
END

-- =============================================================
-- 5. SINH DỮ LIỆU SINH VIÊN CNTT (200 Sinh viên)
-- =============================================================
SET @i = 1;
DECLARE @MSSV VARCHAR(20);
DECLARE @MaLopRandom VARCHAR(20);
DECLARE @DiaChiRandom NVARCHAR(50);
DECLARE @GioiTinh NVARCHAR(10);
DECLARE @NgaySinh DATE;

WHILE @i <= 200
BEGIN
    SET @MSSV = 'SV' + RIGHT('0000' + CAST(@i AS VARCHAR(4)), 4);
    
    SELECT TOP 1 @HoTen = H.Ho + ' ' + D.Dem + ' ' + T.Ten 
    FROM #Ho H CROSS JOIN #Dem D CROSS JOIN #Ten T ORDER BY NEWID();

    IF @HoTen LIKE N'%Thị%' SET @GioiTinh = N'Nữ' ELSE SET @GioiTinh = N'Nam';

    -- Chỉ lấy lớp thuộc danh sách lớp CNTT đã tạo ở trên
    SELECT TOP 1 @MaLopRandom = MaLop FROM Lop ORDER BY NEWID();
    
    SELECT TOP 1 @DiaChiRandom = DC FROM #DiaChi ORDER BY NEWID();
    
    SET @NgaySinh = DATEADD(DAY, ABS(CHECKSUM(NEWID()) % 1500), '2001-01-01');
    SET @SDT = '03' + CAST(CAST(RAND()*100000000 AS INT) AS VARCHAR(10));

    INSERT INTO TaiKhoan VALUES (@MSSV, '123', 'SinhVien');
    INSERT INTO SinhVien VALUES (@MSSV, @HoTen, @NgaySinh, @GioiTinh, LOWER(@MSSV) + '@student.fit.edu.vn', @SDT, @DiaChiRandom, @MaLopRandom);

    SET @i = @i + 1;
END

-- =============================================================
-- 6. SINH LỚP HỌC PHẦN (100 Lớp HP chuyên ngành)
-- =============================================================
SET @i = 1;
DECLARE @MaLHP VARCHAR(20);
DECLARE @MaMonRandom VARCHAR(20);
DECLARE @MaGVRandom VARCHAR(20);
DECLARE @Thu INT;
DECLARE @TietBD INT;
DECLARE @Phong NVARCHAR(20);

WHILE @i <= 100
BEGIN
    SET @MaLHP = 'LHP' + RIGHT('000' + CAST(@i AS VARCHAR(3)), 3);
    
    -- Chỉ lấy môn trong danh sách 50 môn CNTT
    SELECT TOP 1 @MaMonRandom = MaMH FROM MonHoc ORDER BY NEWID();
    SELECT TOP 1 @MaGVRandom = MaGV FROM GiangVien ORDER BY NEWID();
    
    SET @Thu = ABS(CHECKSUM(NEWID()) % 6) + 2; 
    SET @TietBD = ABS(CHECKSUM(NEWID()) % 10) + 1;
    -- Phòng máy thực hành CNTT (PM) hoặc Giảng đường (GĐ)
    IF (ABS(CHECKSUM(NEWID()) % 2) = 0)
        SET @Phong = 'PM' + CAST(ABS(CHECKSUM(NEWID()) % 10) + 1 AS VARCHAR);
    ELSE
        SET @Phong = 'GĐ' + CAST(ABS(CHECKSUM(NEWID()) % 20) + 100 AS VARCHAR);

    INSERT INTO LopHocPhan (MaLHP, MaMon, MaGV, HocKy, NamHoc, TyLeQT, TyLeCK, Thu, TietBD, SoTiet, Phong)
    VALUES (@MaLHP, @MaMonRandom, @MaGVRandom, '1', '2024-2025', 0.4, 0.6, @Thu, @TietBD, 3, @Phong);

    SET @i = @i + 1;
END

-- =============================================================
-- 7. SINH KẾT QUẢ (500 Điểm)
-- =============================================================
DECLARE @Count INT = 0;
DECLARE @MaxRecords INT = 500;

WHILE @Count < @MaxRecords
BEGIN
    DECLARE @MaLHPRan VARCHAR(20);
    DECLARE @MSSVRan VARCHAR(20);
    
    SELECT TOP 1 @MaLHPRan = MaLHP FROM LopHocPhan ORDER BY NEWID();
    SELECT TOP 1 @MSSVRan = MSSV FROM SinhVien ORDER BY NEWID();

    IF NOT EXISTS (SELECT * FROM KetQua WHERE MaLHP = @MaLHPRan AND MSSV = @MSSVRan)
    BEGIN
        DECLARE @DiemGK FLOAT = CAST(ABS(CHECKSUM(NEWID()) % 101) AS FLOAT) / 10;
        DECLARE @DiemCK FLOAT = CAST(ABS(CHECKSUM(NEWID()) % 101) AS FLOAT) / 10;
        
        -- Ngành CNTT thường có tỷ lệ 40-60 hoặc 30-70. Ở đây tính theo LHP đã random
        -- Nhưng để đơn giản tính trung bình theo trọng số đã set ở LHP
        DECLARE @TyLeQT FLOAT;
        DECLARE @TyLeCK FLOAT;
        SELECT @TyLeQT = TyLeQT, @TyLeCK = TyLeCK FROM LopHocPhan WHERE MaLHP = @MaLHPRan;
        
        DECLARE @DiemTK FLOAT = ROUND(@DiemGK * @TyLeQT + @DiemCK * @TyLeCK, 1);
        DECLARE @DiemChu VARCHAR(5);

        SET @DiemChu = CASE 
            WHEN @DiemTK >= 9.0 THEN 'A+'
            WHEN @DiemTK >= 8.5 THEN 'A'
            WHEN @DiemTK >= 8.0 THEN 'B+'
            WHEN @DiemTK >= 7.0 THEN 'B'
            WHEN @DiemTK >= 6.5 THEN 'C+'
            WHEN @DiemTK >= 5.5 THEN 'C'
            WHEN @DiemTK >= 5.0 THEN 'D+'
            WHEN @DiemTK >= 4.0 THEN 'D'
            ELSE 'F'
        END

        INSERT INTO KetQua (MaLHP, MSSV, DiemGK, DiemCK, DiemTK, DiemChu)
        VALUES (@MaLHPRan, @MSSVRan, @DiemGK, @DiemCK, @DiemTK, @DiemChu);
        
        SET @Count = @Count + 1;
    END
END

-- =============================================================
-- 8. DỌN DẸP
-- =============================================================
DROP TABLE #Ho;
DROP TABLE #Dem;
DROP TABLE #Ten;
DROP TABLE #DiaChi;

ALTER TABLE KetQua CHECK CONSTRAINT ALL;
ALTER TABLE LopHocPhan CHECK CONSTRAINT ALL;
ALTER TABLE SinhVien CHECK CONSTRAINT ALL;
ALTER TABLE GiangVien CHECK CONSTRAINT ALL;
ALTER TABLE Lop CHECK CONSTRAINT ALL;
ALTER TABLE MonHoc CHECK CONSTRAINT ALL;
ALTER TABLE Khoa CHECK CONSTRAINT ALL;
ALTER TABLE TaiKhoan CHECK CONSTRAINT ALL;

GO
SELECT N'Đã khởi tạo dữ liệu ngành CNTT thành công!' as ThongBao;
SELECT 'SV: ' + CAST(COUNT(*) AS VARCHAR) FROM SinhVien;
SELECT 'GV: ' + CAST(COUNT(*) AS VARCHAR) FROM GiangVien;
SELECT 'MonHoc: ' + CAST(COUNT(*) AS VARCHAR) FROM MonHoc;
SELECT 'Diem: ' + CAST(COUNT(*) AS VARCHAR) FROM KetQua;