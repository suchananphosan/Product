แบบทดสอบปฎิบัติ
สร้างและออกแบบ Application 1 หน้า ตั้งค่ารหัสสินค้า การทำงานหลักของหน้าจอคือ ค้นหา/เพิ่ม/แก้ไข/ยกเลิก ข้อมูลใน Table.ProductMaster ได้

Tech Stack : 
1. Backend: C# ASP.NET Core 8.0 (MVC)
2. Frontend: Razor Pages, Bootstrap 5.3, jQuery AJAX
3. Database: SQL Server (Entity Framework Core)
4. Library: SweetAlert2, Bootstrap Icons

Features : 
1. ค้นหาสินค้า พิมพ์รหัสสินค้าหรือชื่อสินค้าในช่องค้นหาเพื่อค้นหาสินค้าได้ง่ายยิ่งขึ้น และหากข้อมูลมีจำนวนมากจะถูกแบ่งหน้าให้อัตโนมัติ
2. เพิ่มสินค้า กดปุ่มเพิ่มสินค้าใหม่ และกรอกข้อมูล ได้แก่ รหัสสินค้า , ชื่อสินค้า , ประเภทสินค้า โดยระบบจะตั้งค่าสถานะให้เป็นใช้งานโดยอัตโนมัติ และกดปุ่มบันทึกเพื่อเพิ่มสินค้า
3. แก้ไขสินค้า กดปุ่มแก้ไขสินค้าที่ต้องการ เมื่อแก้ไขข้อมูลสินค้าเสร็จ กดบันทึกเพื่ออัปเดตข้อมูล
4. ยกเลิกสินค้า กดปุ่มลบหรือไอคอนถังขยะเพื่อเปลี่ยนสถานะใช้งานเป็นสถานะยกเลิก เมื่อกดยืนยันยกเลิก รายการข้อมูลสินค้าจะหายไปจากรายการสินค้า แต่ในฐานข้อมูลจะไม่ถูกลบ
5.  แสดงรายการสินค้า

How to Run :
1. Clone project จาก Repository นี้
2. เปิดไฟล์ ProductApp.csproj 
3. Database Setup: แก้ไข Connection String ใน appsettings.json ให้ตรงกับ SQL Server ของคุณ และExecute ไฟล์ db_setup.sql ใน SSMS เพื่อสร้างฐานข้อมูลและตาราง
4. กดรันโปรแกรม Visual Studio 2022 หรือ F5 

Screenshots :
<img width="1919" height="892" alt="image" src="https://github.com/user-attachments/assets/7adfbe91-e054-4c27-b934-9cc89fb6612f" />




