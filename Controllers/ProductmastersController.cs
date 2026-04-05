using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductApp.Models;

namespace ProductApp.Controllers
{
    public class ProductmastersController : Controller
    {
        private readonly ProductContext _context;

        public ProductmastersController(ProductContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString, int? pageNumber)
        {
            int pageSize = 10;
            var products = from p in _context.Productmasters select p;

            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(s =>
                    s.Productname.Contains(searchString) ||
                    s.Productid.Contains(searchString));
            }

            products = products.OrderByDescending(p => p.Createdate);

            int count = await products.CountAsync();
            int currentPage = (count > 0) ? (pageNumber ?? 1) : 0; 
            int totalPages = (int)Math.Ceiling(count / (double)pageSize);
            int displayTotalPages = (count > 0) ? totalPages : 0; 

            var items = await products
                .Skip((Math.Max(1, currentPage) - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = currentPage;
            ViewBag.TotalPages = displayTotalPages;
            ViewBag.TotalCount = count;
            ViewBag.SearchString = searchString;
            ViewBag.HasPreviousPage = currentPage > 1;
            ViewBag.HasNextPage = currentPage < totalPages;

            return View(items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Productid,Productname,Producttypeid,Productstatus,Createuser")] Productmaster productmaster)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "ข้อมูลไม่ถูกต้อง กรุณาตรวจสอบอีกครั้ง";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // เช็ค Primary Key ซ้ำ
                bool isDuplicate = await _context.Productmasters.AnyAsync(e => e.Productid == productmaster.Productid);
                if (isDuplicate)
                {
                    TempData["Error"] = "ไม่สามารถเพิ่มได้เนื่องจากรหัสสินค้าซ้ำ";
                    return RedirectToAction(nameof(Index));
                }

                productmaster.Createdate = DateTime.Now;
                _context.Productmasters.Add(productmaster);
                await _context.SaveChangesAsync();

                TempData["Success"] = "เพิ่มข้อมูลสินค้าสำเร็จ";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "เกิดข้อผิดพลาด: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Productid,Productname,Producttypeid,Productstatus,Updateuser")] Productmaster productmaster)
        {
            ModelState.Remove("Createuser");
            ModelState.Remove("Createdate");

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "ข้อมูลไม่ถูกต้อง";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var existing = await _context.Productmasters.AsNoTracking().FirstOrDefaultAsync(p => p.Productid == id);
                if (existing == null)
                {
                    TempData["Error"] = "ไม่พบข้อมูลที่ต้องการแก้ไข";
                    return RedirectToAction(nameof(Index));
                }

                bool isIdChanged = !string.Equals(id?.Trim(), productmaster.Productid?.Trim(), StringComparison.OrdinalIgnoreCase);

                if (isIdChanged)
                {
                    bool isDuplicate = await _context.Productmasters.AnyAsync(p => p.Productid == productmaster.Productid);
                    if (isDuplicate)
                    {
                        TempData["Error"] = "รหัสสินค้าใหม่ซ้ำกับที่มีอยู่ในระบบ";
                        return RedirectToAction(nameof(Index));
                    }

                    using var transaction = await _context.Database.BeginTransactionAsync();
                    try
                    {
                        var oldData = await _context.Productmasters.FindAsync(id);
                        _context.Productmasters.Remove(oldData);
                        await _context.SaveChangesAsync();

                        productmaster.Createdate = existing.Createdate;
                        productmaster.Createuser = existing.Createuser;
                        productmaster.Updatedate = DateTime.Now;
                        _context.Productmasters.Add(productmaster);
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();
                        TempData["Success"] = "แก้ไขและเปลี่ยนรหัสสินค้าสำเร็จ";
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
                else
                {
                    productmaster.Createdate = existing.Createdate;
                    productmaster.Createuser = existing.Createuser;
                    productmaster.Updatedate = DateTime.Now;

                    _context.Entry(productmaster).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "แก้ไขข้อมูลสำเร็จ";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> FastDelete(string id)
        {
            try
            {
                var product = await _context.Productmasters.FindAsync(id);
                if (product != null)
                {
                    product.Productstatus = 9; 
                    product.Updateuser = "Suchanan.S";
                    product.Updatedate = DateTime.Now;

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "ยกเลิกการใช้งานสินค้าเรียบร้อย";
                }
                else
                {
                    TempData["Error"] = "ไม่พบข้อมูลสินค้า";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "เกิดข้อผิดพลาด: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}