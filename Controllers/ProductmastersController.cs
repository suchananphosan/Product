using System;
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

        // ===================== INDEX (รวม Search + Pagination) =====================
        public async Task<IActionResult> Index(string searchString, int? pageNumber)
        {
            int pageSize = 10;

            var products = from p in _context.Productmasters select p;

            // 🔍 Search
            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(s =>
                    s.Productname.Contains(searchString) ||
                    s.Productid.Contains(searchString));
            }

            // 📅 Sort
            products = products.OrderByDescending(p => p.Createdate);

            // 📄 Pagination
            int count = await products.CountAsync();
            int currentPage = pageNumber ?? 1;
            int totalPages = (int)Math.Ceiling(count / (double)pageSize);

            var items = await products
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = currentPage;
            ViewBag.TotalPages = totalPages;
            ViewBag.HasPreviousPage = currentPage > 1;
            ViewBag.HasNextPage = currentPage < totalPages;
            ViewBag.SearchString = searchString;

            return View(items);
        }

        // ===================== CREATE =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Productid,Productname,Producttypeid,Productstatus,Createuser")]
            Productmaster productmaster)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "ข้อมูลไม่ถูกต้อง";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                bool isDuplicate = await _context.Productmasters
                    .AnyAsync(e => e.Productid == productmaster.Productid);

                if (isDuplicate)
                {
                    TempData["Error"] = "รหัสสินค้าซ้ำ";
                    return RedirectToAction(nameof(Index));
                }

                productmaster.Createdate = DateTime.Now;

                _context.Productmasters.Add(productmaster);
                await _context.SaveChangesAsync();

                TempData["Success"] = "เพิ่มข้อมูลสำเร็จ";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // ===================== EDIT =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            string id,
            [Bind("Productid,Productname,Producttypeid,Productstatus,Updateuser")]
            Productmaster productmaster)
        {
            // กัน validation field ที่ไม่ได้ส่งมา
            ModelState.Remove("Createuser");
            ModelState.Remove("Createdate");

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "ข้อมูลไม่ถูกต้อง";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var existing = await _context.Productmasters.FindAsync(id);

                if (existing == null)
                {
                    TempData["Error"] = "ไม่พบข้อมูล";
                    return RedirectToAction(nameof(Index));
                }

                // 🔥 เช็คว่าเปลี่ยน ID ไหม
                bool isIdChanged = !string.Equals(
                    id?.Trim(),
                    productmaster.Productid?.Trim(),
                    StringComparison.OrdinalIgnoreCase
                );

                // ===================== เปลี่ยน ID =====================
                if (isIdChanged)
                {
                    bool isDuplicate = await _context.Productmasters
                        .AnyAsync(p => p.Productid == productmaster.Productid);

                    if (isDuplicate)
                    {
                        TempData["Error"] = "รหัสสินค้าใหม่ซ้ำ";
                        return RedirectToAction(nameof(Index));
                    }

                    using var transaction = await _context.Database.BeginTransactionAsync();

                    try
                    {
                        var newData = new Productmaster
                        {
                            Productid = productmaster.Productid,
                            Productname = productmaster.Productname,
                            Producttypeid = productmaster.Producttypeid,
                            Productstatus = productmaster.Productstatus,
                            Createuser = existing.Createuser,
                            Createdate = existing.Createdate,
                            Updateuser = productmaster.Updateuser,
                            Updatedate = DateTime.Now
                        };

                        _context.Productmasters.Remove(existing);
                        await _context.SaveChangesAsync();

                        _context.Productmasters.Add(newData);
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();

                        TempData["Success"] = "แก้ไขสำเร็จ (เปลี่ยนรหัสสินค้า)";
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        TempData["Error"] = "เกิดข้อผิดพลาด";
                    }
                }
                // ===================== ไม่เปลี่ยน ID =====================
                else
                {
                    existing.Productname = productmaster.Productname;
                    existing.Producttypeid = productmaster.Producttypeid;
                    existing.Productstatus = productmaster.Productstatus;
                    existing.Updateuser = productmaster.Updateuser;
                    existing.Updatedate = DateTime.Now;

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

        // ===================== SOFT DELETE =====================
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

                    TempData["Success"] = "ยกเลิกสินค้าเรียบร้อย";
                }
                else
                {
                    TempData["Error"] = "ไม่พบข้อมูล";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}