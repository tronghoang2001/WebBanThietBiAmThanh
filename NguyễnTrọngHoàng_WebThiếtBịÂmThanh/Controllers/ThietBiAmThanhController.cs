﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NguyễnTrọngHoàng_WebThiếtBịÂmThanh.Models;

using PagedList;
using PagedList.Mvc;

namespace NguyễnTrọngHoàng_WebThiếtBịÂmThanh.Controllers
{
    public class ThietBiAmThanhController : Controller
    {
        //Tạo 1 đối tượng chứa toàn bộ CSDL từ dbBanThietBiAmThanh
        dbThietBiAmThanhDataContext data = new dbThietBiAmThanhDataContext();

        private List<SanPham> LaySanPham(int count)
        {
            return data.SanPhams.OrderByDescending(a => a.idSanPham).Take(count).ToList();
        }
        // GET: ThietBiAmThanh
        public ActionResult Index(int ? page)
        {
            //Tạo biến quy định số sản phẩm trên mỗi trang 
            int pageSize = 4;
            //Tạo biến số trang
            int pageNum = (page ?? 1);
            //lấy 12 sản phẩm theo id
            var sanphammoi = LaySanPham(12);
            return View(sanphammoi.ToPagedList(pageNum,pageSize));
        }
        public ActionResult LoaiSanPham()
        {
            var loaisanpham = from lsp in data.LoaiSanPhams select lsp;
            return PartialView(loaisanpham);
        }
        public ActionResult ThuongHieu()
        {
            var thuonghieu = from th in data.ThuongHieus select th;
            return PartialView(thuonghieu);
        }
        public ActionResult SPTheoLoai(int id)
        {
            var sanpham = from sp in data.SanPhams where sp.idLoai == id select sp;
            return View(sanpham);
        }
        public ActionResult SPTheoThuongHieu(int id)
        {
            var sanpham = from sp in data.SanPhams where sp.idThuongHieu == id select sp;
            return View(sanpham);
        }
        public ActionResult Details(int id)
        {
            var sanpham = from sp in data.SanPhams where sp.idSanPham == id select sp;
            return View(sanpham.Single());
        }
    }
}