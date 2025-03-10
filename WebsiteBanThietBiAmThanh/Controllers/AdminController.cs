﻿using WebsiteBanThietBiAmThanh.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using System.IO;
using PagedList;
using PagedList.Mvc;
using System.IO;

namespace WebsiteBanThietBiAmThanh.Controllers
{
    public class AdminController : Controller
    {
        dbThietBiAmThanhDataContext db = new dbThietBiAmThanhDataContext();
        // GET: Admin
        public ActionResult Index()
        {
            //CHECK SESSION
            if (Session["Taikhoanadmin"] != null)
            {
                return View();
            }
            else
            {
                //Login:hàm, admin:Controller
                return RedirectToAction("Login", "Admin");
            }
        }
        public ActionResult  Sanpham(int ? page)
         {
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            //return View(db.SanPhams.ToList());
           return View(db.SanPhams.ToList().OrderBy(n=>n.idSanPham).ToPagedList(pageNumber,pageSize));
        }

        public static string CreateMD5(string matkhau)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(matkhau);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                return Convert.ToBase64String(hashBytes); // .NET 5 +

                //string s = Convert.ToBase64String(hashBytes);
                //return s;
            }
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            //gán các giá trị người dùng nhập liệu cho các biến
            var tendn = collection["username"];
            var matkhau = collection["password"];
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Phải nhập tên đăng nhập";

            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Phải nhập mật khẩu";
            }
            else
            {
                //gán  giá trị cho đối tượng được tạo mới(ad)
                Admin nv = db.Admins.SingleOrDefault(n => n.UserAdmin == tendn && n.PassAdmin == CreateMD5(matkhau));
                if (nv != null)
                {
                    //ViewBag.ThongBao = "Chúc mừng đăng nhập thành công";
                    Session["Taikhoanadmin"] = nv;
                    return RedirectToAction("Index", "Admin");
                }
                else
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";
            }
            return View();

        }

        public ActionResult LogOut()
        {
            Session["Taikhoanadmin"] = null;
            Session.Clear();
            return RedirectToAction("Login", "Admin");
        }

        //THÊM MỚI SẢN PHẨM
        [HttpGet]
        public ActionResult Themmoisanpham()
        {

            //dua du lieu vao dropdownlist
            //lay ds tu table chu de, sap xep tang dan theo ten chu de, chon lay gia tri ma CD, hien thi ten CD
            ViewBag.idLoai = new SelectList(db.LoaiSanPhams.ToList().OrderBy(n => n.tenLoai), "idLoai", "tenLoai");
            ViewBag.idThuongHieu = new SelectList(db.ThuongHieus.ToList().OrderBy(n => n.hinhAnhTH), "idThuongHieu", "hinhAnhTH");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Themmoisanpham(SanPham sanPham, HttpPostedFileBase fileUpload)
        {
            ViewBag.idLoai = new SelectList(db.LoaiSanPhams.ToList().OrderBy(n => n.tenLoai), "idLoai", "tenLoai");
            ViewBag.idThuongHieu = new SelectList(db.ThuongHieus.ToList().OrderBy(n => n.hinhAnhTH), "idThuongHieu", "hinhAnhTH");

            if (fileUpload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh ";
                return View();
            }
            else
            {
                if (ModelState.IsValid)
                //luu ten file
                {
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    //luu duong dan cua file
                    var path = Path.Combine(Server.MapPath("~/Content/images"), fileName);
                    //kiem tra hinh anh ton tai chua
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    }
                    else
                    {
                        // luu hinh anh vao duong dan
                        fileUpload.SaveAs(path);
                    }
                    sanPham.hinhAnhSP = fileName;
                    //luu vao CSDL
                    db.SanPhams.InsertOnSubmit(sanPham);
                    db.SubmitChanges();
                }


                return RedirectToAction("Sanpham");

            }
        }

        //HIỂN THỊ SẢN PHẨM

        public ActionResult Chitietsanpham(int id)
        {
            SanPham sanPham = db.SanPhams.SingleOrDefault(n => n.idSanPham == id);
            ViewBag.idSanPham = sanPham.idSanPham;
            if (sanPham == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sanPham);
        }
        //XÓA SẢN PHẨM
        [HttpGet]
        public ActionResult Xoasanpham(int id)
        {
            SanPham sanPham = db.SanPhams.SingleOrDefault(n => n.idSanPham == id);
            ViewBag.idSanPham = sanPham.idSanPham;
            if (sanPham == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sanPham);

        }
        [HttpPost, ActionName("Xoasanpham")]
        public ActionResult Xacnhanxoa(int id)
        {
            //LẤY RA ĐỐI TƯỢNG SẢN PHẨM THEO MÃ
            SanPham sanPham = db.SanPhams.SingleOrDefault(n => n.idSanPham == id);
            ViewBag.idSanPham = sanPham.idSanPham;
            if (sanPham == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.SanPhams.DeleteOnSubmit(sanPham);
            db.SubmitChanges();
            return RedirectToAction("SanPham");
        }


        //CHỈNH SỬA SẢN PHẨM
        [HttpGet]
        public ActionResult Suasanpham(int id)
        {
            //LẤY RA ĐỐI TƯỢNG SẢN PHẨM THEO MÃ
            SanPham sanPham = db.SanPhams.SingleOrDefault(n => n.idSanPham == id);
            if (sanPham == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.idLoai = new SelectList(db.LoaiSanPhams.ToList().OrderBy(n => n.tenLoai), "idLoai", "tenLoai");
            ViewBag.idThuongHieu = new SelectList(db.ThuongHieus.ToList().OrderBy(n => n.hinhAnhTH), "idThuongHieu", "hinhAnhTH");
            return View(sanPham);
        }



        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Suasanpham(int id, FormCollection collection, HttpPostedFileBase fileUpload)
        {
            var sanpham = db.SanPhams.FirstOrDefault(n => n.idSanPham == id);
            sanpham.idSanPham = id;
            //Dua du lieu vao dropdownload
            ViewBag.idLoai = new SelectList(db.LoaiSanPhams.ToList().OrderBy(n => n.tenLoai), "idLoai", "tenLoai");
            ViewBag.idThuongHieu = new SelectList(db.ThuongHieus.ToList().OrderBy(n => n.hinhAnhTH), "idThuongHieu", "hinhAnhTH");
            //kiem tra duong dan file
            if (fileUpload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                return View(sanpham);
            }
            //Them vao CSDL
            else
            {
                if (ModelState.IsValid)
                {
                    //luu ten file
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    //luu duong dan cua file
                    var path = Path.Combine(Server.MapPath("~/Content/images"), fileName);
                    //kiem tra hinh anh ton tai chua
                    if (System.IO.File.Exists(path))
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    else
                    {
                        // luu hinh anh vao duong dan
                        fileUpload.SaveAs(path);
                    }
                    sanpham.hinhAnhSP = fileName;
                    sanpham.idSanPham = id;
                    //luu vao CSDL
                    UpdateModel(sanpham);
                    db.SubmitChanges();
                    return RedirectToAction("SanPham");
                }
                return this.Suasanpham(id);

            }
        }

        //LOẠI SẢN PHẨM
        public ActionResult LoaiSanPham()
        {
            return View(db.LoaiSanPhams.ToList());
        }
        //THÊM MỚI LOẠI SẢN PHẨM
        [HttpGet]
        public ActionResult ThemMoiLoaiSanPham()
        {
            ViewBag.idLoai = new SelectList(db.LoaiSanPhams.ToList().OrderBy(n => n.tenLoai), "idLoai", "tenLoai");

            return View();
        }
        [HttpPost]
        public ActionResult ThemMoiLoaiSanPham(LoaiSanPham loaiSanPham, HttpPostedFileBase fileUpload)
        {
            ViewBag.idLoai = new SelectList(db.LoaiSanPhams.ToList().OrderBy(n => n.tenLoai), "idLoai", "tenLoai");
            //luu vao CSDL
            db.LoaiSanPhams.InsertOnSubmit(loaiSanPham);
            db.SubmitChanges();
            return View();
        }
        //Xóa loại sản phẩm
        [HttpGet]
        public ActionResult XoaLoaiSanPham(int id)
        {

            //Lấy đối tượng cần xóa theo mã
            LoaiSanPham loaiSanPham = db.LoaiSanPhams.SingleOrDefault(n => n.idLoai == id);
            ViewBag.idLoai = loaiSanPham.idLoai;
            if (loaiSanPham == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(loaiSanPham);

        }
        [HttpPost, ActionName("XoaLoaiSanPham")]
        public ActionResult Xacnhanxoa1(int id)
        {
            //Lay ra doi tuong can xoa theo ma
            LoaiSanPham loaiSanPham = db.LoaiSanPhams.SingleOrDefault(n => n.idLoai == id);
            ViewBag.idLoai = loaiSanPham.idLoai;

            if (loaiSanPham == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.LoaiSanPhams.DeleteOnSubmit(loaiSanPham);
            db.SubmitChanges();
            return RedirectToAction("LoaiSanPham");
        }

        // Chỉnh sửa thông tin loại sản phẩm
        [HttpGet]
        public ActionResult SuaLoaiSanPham(int id)
        {
            //lay ra doi tuong san pham theo ma
            LoaiSanPham loaiSanPham = db.LoaiSanPhams.SingleOrDefault(n => n.idLoai == id);

            if (loaiSanPham == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            ViewBag.idLoai = new SelectList(db.LoaiSanPhams.ToList().OrderBy(n => n.tenLoai), "idLoai", "tenLoai");
            return View(loaiSanPham);
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SuaLoaiSanPham(int id, FormCollection collection, HttpPostedFileBase fileUpload)
        {
            LoaiSanPham loaiSanPham = db.LoaiSanPhams.SingleOrDefault(n => n.idLoai == id);
            ViewBag.idLoai = loaiSanPham.idLoai;

            //luu vao CSDL
            UpdateModel(loaiSanPham);
            db.SubmitChanges();
            return RedirectToAction("LoaiSanPham");
        }
        //THƯƠNG HIỆU
        public ActionResult ThuongHieuSanPham()
        {
            return View(db.ThuongHieus.ToList());
        }
        public ActionResult XemBanDo()
        {
            return View();
        }
        public ActionResult XemThoiTiet()
        {
            return View();
        }

        //khách hang
        public ActionResult Khach(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            return View(db.KhachHangs.ToList().OrderBy(n => n.idKhachHang).ToPagedList(pageNumber, pageSize));
        }
        //chi tiet khách hàng
        public ActionResult ChiTietKhach(int id)
        {
            //lấy ra đối tượng sách theo mã
            KhachHang kh = db.KhachHangs.SingleOrDefault(n => n.idKhachHang == id);
            ViewBag.idKhachHang = kh.idKhachHang;
            if (kh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(kh);
        }
        //xóa khách
        [HttpGet]
        public ActionResult XoaKhach(int id)
        {
            //lấy ra đối tượng khách cần xóa
            KhachHang kh = db.KhachHangs.SingleOrDefault(n => n.idKhachHang == id);
            ViewBag.idKhachHang = kh.idKhachHang;
            if (kh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(kh);
        }
        [HttpPost, ActionName("XoaKhach")]
        public ActionResult XacNhanXoaKhach(int id)
        {
            //lấy ra đối tượng khách cần xóa
            KhachHang kh = db.KhachHangs.SingleOrDefault(n => n.idKhachHang == id);
            ViewBag.idKhachHang = kh.idKhachHang;
            if (kh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.KhachHangs.DeleteOnSubmit(kh);
            db.SubmitChanges();
            return RedirectToAction("Khach");
        }


        //Bình luận
        public ActionResult BinhLuan(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            return View(db.BinhLuans.ToList().OrderBy(n => n.idBinhLuan).ToPagedList(pageNumber, pageSize));
        }
        //xem bình luận chi tiết
        public ActionResult ChiTietBinhLuan(int id)
        {
            //lấy ra đối tượng sách theo mã
            BinhLuan bl = db.BinhLuans.SingleOrDefault(n => n.idBinhLuan == id);
            ViewBag.idBinhLuan = bl.idBinhLuan;
            if (bl == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(bl);
        }
        //xóa khách
        [HttpGet]
        public ActionResult XoaBinhLuan(int id)
        {
            //lấy ra đối tượng khách cần xóa
            BinhLuan bl = db.BinhLuans.SingleOrDefault(n => n.idBinhLuan == id);
            ViewBag.idBinhLuan = bl.idBinhLuan;
            if (bl == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(bl);
        }
        [HttpPost, ActionName("XoaBinhLuan")]
        public ActionResult XacNhanXoaBinhLuan(int id)
        {
            //lấy ra đối tượng khách cần xóa
            BinhLuan bl = db.BinhLuans.SingleOrDefault(n => n.idBinhLuan == id);
            ViewBag.idBinhLuan = bl.idBinhLuan;
            if (bl == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.BinhLuans.DeleteOnSubmit(bl);
            db.SubmitChanges();
            return RedirectToAction("BinhLuan");
        }



        //Liên hệ
        public ActionResult LienHe(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            return View(db.LienHes.ToList().OrderBy(n => n.idlienHe).ToPagedList(pageNumber, pageSize));
        }
        //xem liên hệ chi tiết
        public ActionResult ChiTietLienHe(int id)
        {
            //lấy ra đối tượng sách theo mã
            LienHe lh = db.LienHes.SingleOrDefault(n => n.idlienHe == id);
            ViewBag.idlienHe = lh.idlienHe;
            if (lh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(lh);
        }
        [HttpGet]
        public ActionResult XoaLienHe(int id)
        {
            //lấy ra đối tượng khách cần xóa
            LienHe lh = db.LienHes.SingleOrDefault(n => n.idlienHe == id);
            ViewBag.idlienHe = lh.idlienHe;
            if (lh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(lh);
        }
        [HttpPost, ActionName("XoaLienHe")]
        public ActionResult XacNhanXoaLienHe(int id)
        {
            //lấy ra đối tượng khách cần xóa
            LienHe lh = db.LienHes.SingleOrDefault(n => n.idlienHe == id);
            ViewBag.idlienHe = lh.idlienHe;
            if (lh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.LienHes.DeleteOnSubmit(lh);
            db.SubmitChanges();
            return RedirectToAction("LienHe");
        }

        ////Tài khoản
        //public ActionResult TaiKhoanAdmin(int? page)
        //{
        //    int pageNumber = (page ?? 1);
        //    int pageSize = 7;
        //    return View(db.Admins.ToList().OrderBy(n => n.idAdmin).ToPagedList(pageNumber, pageSize));
        //}


        //[HttpGet]
        //public ActionResult ThemMoiAdmin()
        //{
        //    return View();
        //}
        //[HttpPost]
        //public ActionResult ThemMoiAdmin(Admin ad, FormCollection collection)
        //{
        //    //Gán các giá trị người dùng nhập liệu cho các biến
        //    var hoten = collection["Hoten"];
        //    var tendn = collection["UserAdmin"];
        //    var matkhau = CreateMD5(collection["PassAdmin"]);
        //    if (String.IsNullOrEmpty(hoten))
        //    {
        //        ViewData["Loi1"] = "Họ tên không được trống";
        //    }
        //    if (String.IsNullOrEmpty(tendn))
        //    {
        //        ViewData["Loi2"] = "Tên đăng nhập không được trống";
        //    }
        //    if (String.IsNullOrEmpty(matkhau))
        //    {
        //        ViewData["Loi3"] = "Mật khẩu không được trống";
        //    }
        //    else
        //    {
        //        ad.Hoten = hoten;
        //        ad.UserAdmin = tendn;
        //        ad.PassAdmin = CreateMD5(matkhau);
        //        db.Admins.InsertOnSubmit(ad);
        //        db.SubmitChanges();
        //        return RedirectToAction("TaiKhoanAdmin");
        //    }
        //    return this.ThemMoiAdmin();
        //}
        ////Xóa tin tức
        //[HttpGet]
        //public ActionResult XoaAdmin(int id)
        //{
        //    var ad = db.Admins.FirstOrDefault(n => n.idAdmin == id);
        //    ad.idAdmin = id;
        //    if (ad == null)
        //    {
        //        Response.StatusCode = 404;
        //        return null;
        //    }
        //    return View(ad);

        //}
        //[HttpPost, ActionName("XoaAdmin")]
        //public ActionResult XacNhanXoaAdmin(int id)
        //{
        //    //Lấy đối tượng tin tức theo mã
        //    var ad = db.Admins.SingleOrDefault(n => n.idAdmin == id);
        //    ad.idAdmin = id;
        //    if (ad == null)
        //    {
        //        Response.StatusCode = 404;
        //        return null;
        //    }
        //    db.Admins.DeleteOnSubmit(ad);
        //    db.SubmitChanges();
        //    return RedirectToAction("TaiKhoanAdmin");
        //}

        //// Chỉnh sửa thông tin loại sản phẩm
        //[HttpGet]
        //public ActionResult SuaAdmin(int id)
        //{
        //    //lay ra doi tuong admin theo ma
        //    var ad = db.Admins.SingleOrDefault(n => n.idAdmin == id);

        //    if (ad == null)
        //    {
        //        Response.StatusCode = 404;
        //        return null;
        //    }
        //    return View(ad);
        //}


        //[HttpPost]
        //[ValidateInput(false)]
        //public ActionResult SuaAdmin(int id, FormCollection collection)
        //{
        //    //Tạo 1 biến khachhang với đối tương id = id truyền vào
        //    var ad = db.Admins.First(n => n.idAdmin == id);
        //    var matkhaumoi = CreateMD5(collection["NewPassAdmin"]);
        //    var hoten = collection["Hoten"];
        //    ad.idAdmin = id;
        //    //Nếu người dùng không nhập mk mới và nhập lại mk mới
        //    if (string.IsNullOrEmpty(matkhaumoi))
        //    {
        //        ViewData["Loi1"] = "Chưa nhập mật khẩu mới!";
        //    }
        //    if (string.IsNullOrEmpty(hoten))
        //    {
        //        ViewData["Loi2"] = "Chưa nhập lại họ tên mới!";
        //    }
        //    else
        //    {
        //        ad.Hoten = hoten;
        //        ad.PassAdmin = CreateMD5(matkhaumoi);
        //        //Update trong CSDL
        //        UpdateModel(ad);
        //        db.SubmitChanges();
        //        return RedirectToAction("TaiKhoanAdmin");
        //    }
        //    return this.SuaAdmin(id);
        //}


        //Tin tức
        public ActionResult TinTuc(int ?page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            //return View(db.TinTucs.ToList());
            return View(db.TinTucs.ToList().OrderBy(n => n.idTinTuc).ToPagedList(pageNumber, pageSize));
        }
        
        [HttpGet]
        public ActionResult ThemTinTuc()
        {
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemTinTuc(TinTuc tintuc, HttpPostedFileBase fileupload)
        {
            //Kiểm tra đường dẫn file
            if (fileupload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                return View();
            }
            //Thêm vào CSDL
            else
            {
                if (ModelState.IsValid)
                {
                    //Lưu tên file, lưu ý bổ sung thư viện using System.IO;
                    var fileName = Path.GetFileName(fileupload.FileName);
                    //Lưu đường dẫn của file
                    var path = Path.Combine(Server.MapPath("~/Content/images"), fileName);
                    //Kiểm tra hình ảnh tồn tại chưa?
                    if (System.IO.File.Exists(path))
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    else
                    {
                        //Lưu hình ảnh vào đường dẫn
                        fileupload.SaveAs(path);
                    }
                    tintuc.anhBiaTin = fileName;
                    //Lưu vào CSDL
                    db.TinTucs.InsertOnSubmit(tintuc);
                    db.SubmitChanges();
                }
                return RedirectToAction("TinTuc");
            }
        }
        public ActionResult ChiTietTinTuc(int id)
        {
            TinTuc tintuc = db.TinTucs.SingleOrDefault(n => n.idTinTuc == id);
            ViewBag.idTinTuc = tintuc.idTinTuc;
            if (tintuc == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(tintuc);
        }

        //Xóa tin tức
        [HttpGet]
        public ActionResult XoaTinTuc(int id)
        {
            TinTuc tintuc = db.TinTucs.SingleOrDefault(n => n.idTinTuc == id);
            ViewBag.idTinTuc = tintuc.idTinTuc;
            if (tintuc == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(tintuc);

        }
        [HttpPost,ActionName("XoaTinTuc")]
        public ActionResult XacNhanXoaTinTuc(int id)
        {
            //Lấy đối tượng tin tức theo mã
            TinTuc tintuc = db.TinTucs.SingleOrDefault(n => n.idTinTuc == id);
            ViewBag.idTinTuc = tintuc.idTinTuc;
            if (tintuc == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.TinTucs.DeleteOnSubmit(tintuc);
            db.SubmitChanges();
            return RedirectToAction("TinTuc");
        }

        //Sửa tin tức
        [HttpGet]
        public ActionResult SuaTinTuc(int id)
        {
            //Lấy ra đối tượng tin tức theo mã
            TinTuc tintuc = db.TinTucs.SingleOrDefault(n => n.idTinTuc == id);
            if (tintuc == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(tintuc);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SuaTinTuc(int id, HttpPostedFileBase fileUpload)
        {
            var tintuc = db.TinTucs.FirstOrDefault(n => n.idTinTuc == id);
            tintuc.idTinTuc = id;
            //kiem tra duong dan file
            if (fileUpload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                return View(tintuc);
            }
            //Them vao CSDL
            else
            {
                if (ModelState.IsValid)
                {
                    //luu ten file
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    //luu duong dan cua file
                    var path = Path.Combine(Server.MapPath("~/Content/images"), fileName);
                    //kiem tra hinh anh ton tai chua
                    if (System.IO.File.Exists(path))
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    else
                    {
                        // luu hinh anh vao duong dan
                        fileUpload.SaveAs(path);
                    }
                    tintuc.anhBiaTin = fileName;
                    tintuc.idTinTuc = id;
                    //luu vao CSDL
                    UpdateModel(tintuc);
                    db.SubmitChanges();
                    return RedirectToAction("TinTuc");
                }
                return this.SuaTinTuc(id);

            }
        }

        //Đơn hàng
        public ActionResult DonHang(int? page)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return RedirectToAction("Login", "Admin");
            }
            int pageNumber = (page ?? 1);
            int pageSize = 30;
            //return View(db.TinTucs.ToList
            return View(db.DonHangs.ToList().OrderBy(n => n.idDonHang).ToPagedList(pageNumber, pageSize));
        }

        //Xóa tin tức
        [HttpGet]
        public ActionResult XoaDonHang(int id)
        {
            DonHang donhang = db.DonHangs.SingleOrDefault(n => n.idDonHang == id);
            ViewBag.idDonHang = donhang.idDonHang;
            if (donhang == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(donhang);
        }
        [HttpPost, ActionName("XoaDonHang")]
        public ActionResult XacNhanXoaDonHang(int id)
        {
            //Lấy đối tượng tin tức theo mã
            DonHang donhang = db.DonHangs.SingleOrDefault(n => n.idDonHang == id);
            CTDH cthd = db.CTDHs.SingleOrDefault(n => n.idDonHang == id);
            ViewBag.idDonHang = donhang.idDonHang;
            if (donhang == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.CTDHs.DeleteOnSubmit(cthd);
            db.DonHangs.DeleteOnSubmit(donhang);
            db.SubmitChanges();
            return RedirectToAction("DonHang");
        }
    }
}
