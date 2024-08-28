using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace BT01
{
    public partial class Form1 : Form
    {
        //khao bao doi tuong
        DataSet ds = new DataSet();//tuong duong voi database
        DataTable tblKhoa = new DataTable("KHOA");
        DataTable tblSinhVien = new DataTable("SINHVIEN");
        DataTable tblKetQua = new DataTable("KETQUA");
        int stt = -1;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Tao_Cau_Truc_Cac_Bang();
            Moc_Noi_Quan_He_Cac_Bang();
            Nhap_Lieu_Cac_Mang();
            Khoi_Tao_Comobo_Khoa();
            btndau.PerformClick();
        }
        private double TongDiem(String MSV)
        {
            double kq = 0;
            object tb = tblKetQua.Compute("sum(Diem)", "MaSV='" + MSV + "'");
            //lưu ý trường hợp SV không có điểm thì phơng thức compute trả về giá trị DBNull
            if (tb == DBNull.Value)
                kq = 0;
            else
                kq = Convert.ToDouble(tb);
            return kq;
        }
        private void Khoi_Tao_Comobo_Khoa()
        {
            cbcKhoa.DisplayMember = "TenKH";
            cbcKhoa.ValueMember = "MaKhoa";
            cbcKhoa.DataSource = tblKhoa;
        }

        private void Tao_Cau_Truc_Cac_Bang()
        {
            //Tạo cấu trúc cho datatable tương ứng với bảng Khoa
            tblKhoa.Columns.Add("MaKH", typeof(string));
            tblKhoa.Columns.Add("TenKH", typeof(string));
            //Tạo Khóa Chính cho tblKHOA
            tblKhoa.PrimaryKey = new DataColumn[] { tblKhoa.Columns["MaKH"] };
            //Tạo cấu trúc cho datatable tương ứng với bảng SinhVien
            tblSinhVien.Columns.Add("MaSV", typeof(String));
            tblSinhVien.Columns.Add("HoSV", typeof(String));
            tblSinhVien.Columns.Add("TenSV", typeof(String));
            tblSinhVien.Columns.Add("Phai", typeof(Boolean));
            tblSinhVien.Columns.Add("NgaySinh", typeof(DateTime));
            tblSinhVien.Columns.Add("NoiSinh", typeof(String));
            tblSinhVien.Columns.Add("MaKH", typeof(String));
            tblSinhVien.Columns.Add("HocBong", typeof(double));
            //Tạo Khóa Chính cho tblSINHVIEN
            tblSinhVien.PrimaryKey = new DataColumn[] { tblSinhVien.Columns["MaSV"] };
            //Tạo cấu trúc cho datatable tương ứng với bảng tblKETQUA
            tblKetQua.Columns.Add("MaSV", typeof(String));
            tblKetQua.Columns.Add("MaKH", typeof(String));
            tblKetQua.Columns.Add("dime", typeof(double));
            //Tạo Khóa Chính cho tblKETQUA
            tblKetQua.PrimaryKey = new DataColumn[] { tblKetQua.Columns["MaSV"], tblKetQua.Columns["MaKH"] };

            //Thêm đồng thời nhiều datatable
            ds.Tables.AddRange(new DataTable[] { tblKhoa, tblSinhVien, tblKetQua });
        }
        private void Moc_Noi_Quan_He_Cac_Bang()
        {
            //Tạo quan hệ giữa tblKhoa và tblSinhVien
            ds.Relations.Add("FK_KH_SV", ds.Tables["KHOA"].Columns["MaKH"], ds.Tables["SINHVIEN"].Columns["MaKH"], true);
            //Tạo quan hệ giữa tblSinhVien và tblKetQua
            ds.Relations.Add("FK_SV_KQ", ds.Tables["SINHVIEN"].Columns["MaSV"], ds.Tables["KETQUA"].Columns["MaSV"], true);
            //Loại bỏ Cacase Delete trong các quan hệ
            ds.Relations["FK_KH_SV"].ChildKeyConstraint.DeleteRule = Rule.None;
            ds.Relations["FK_SV_KQ"].ChildKeyConstraint.DeleteRule = Rule.None;

        }
        private void Nhap_Lieu_Cac_Mang()
        {
            NhapLieu_tblKhoa();
            NhapLieu_tblSinhVien();
            NhapLieu_tblKetQua();
        }

        private void NhapLieu_tblKetQua()
        {
            //Nhập liệu cho tblKhoa =>Đọc dữ liệu từ tập tin KETQUA.TXT
            string[] Mang_KQ = File.ReadAllLines(@"..\..\..\DATA\KETQUA.txt");
            foreach (string Chuoi_KQ in Mang_KQ)
            {
                //Tách Chuoi_Kha tành các thành phần tương ứng với các cột trong  tblKetQua
                string[] Mang_Thanh_Phan = Chuoi_KQ.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                //Tạo một dòng mới có cấu trúc của một dòng trong tblKetQua
                DataRow rkq = tblKetQua.NewRow();
                //gán giá trị cho các cột của dòng mới tạo
                for (int i = 0; i < Mang_Thanh_Phan.Length; i++)
                {
                    rkq[i] = Mang_Thanh_Phan[i];
                }

                //thêm dòng vừa tạo vào tblKetQua
                tblKetQua.Rows.Add(rkq);
            }
        }

        private void NhapLieu_tblSinhVien()
        {
            //Nhập liệu cho tblKhoa =>Đọc dữ liệu từ tập tin SINHVIEN.TXT
            string[] Mang_SV = File.ReadAllLines(@"..\..\..\DATA\SINHVIEN.txt");
            foreach (string Chuoi_SV in Mang_SV)
            {
                //Tách Chuoi_Kha tành các thành phần tương ứng với các cột trong tblSinhVien
                string[] Mang_Thanh_Phan = Chuoi_SV.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                //Tạo một dòng mới có cấu trúc của một dòng trong tblSinhVien
                DataRow rsv = tblSinhVien.NewRow();
                //gán giá trị cho các cột của dòng mới tạo
                for (int i = 0; i < Mang_Thanh_Phan.Length; i++)
                {
                    rsv[i] = Mang_Thanh_Phan[i];
                }

                //thêm dòng vừa tạo vào tblSinhvien
                tblSinhVien.Rows.Add(rsv);
            }
        }

        private void NhapLieu_tblKhoa()
        {
            //Nhập liệu cho tblKhoa =>Đọc dữ liệu từ tập tin KHOA.TXT
            string[] Mang_Khoa = File.ReadAllLines(@"..\..\..\DATA\KHOA.txt");
            foreach (string Chuoi_Khoa in Mang_Khoa)
            {
                //Tách Chuoi_Kha tành các thành phần tương ứng với MaKH và TenKH
                string[] Mang_Thanh_Phan = Chuoi_Khoa.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                //Tạo một dòng mới có cấu trúc của một dòng trong tblKhoa
                DataRow rkh = tblKhoa.NewRow();
                //gán giá trị cho các cột của dòng mới tạo
                rkh[0] = Mang_Thanh_Phan[0];
                rkh[1] = Mang_Thanh_Phan[1];
                //thêm dòng vừa tạo vào tblKhoa
                tblKhoa.Rows.Add(rkh);
            }
        }
        public void GanDuLieu(int stt)
        {
            //lấy dòng dữ liệu thứ stt trong tblSinhVien
            DataRow rsv = tblSinhVien.Rows[stt];
            txtMaSV.Text = rsv["MaSV"].ToString();
            txtHoSV.Text = rsv["HoSV"].ToString();
            txtTenSV.Text = rsv["TenSV"].ToString();
            ckbPhai.Checked = (Boolean)rsv["Phai"];
            dateNgaySinh.Text = rsv["NgaySinh"].ToString();
            txtNoiSinh.Text = rsv["NoiSinh"].ToString();
            cbcKhoa.SelectedValue = rsv["MaKH"].ToString();
            txtHocBong.Text = rsv["HocBong"].ToString();
            //thể hiện số thứ tự mẫu tin hiện hành
            lblSTT.Text = (stt + 1) + "/" + tblSinhVien.Rows.Count;
            //Tính Tổng Điểm
            txtTongDiem.Text = TongDiem(txtMaSV.Text).ToString();
        }

        private void btndau_Click(object sender, EventArgs e)
        {
            stt = 0;
            GanDuLieu(stt);
        }

        private void btncuoi_Click(object sender, EventArgs e)
        {
            stt = tblSinhVien.Rows.Count - 1;
            GanDuLieu(stt);
        }

        private void btntruoc_Click(object sender, EventArgs e)
        {
            if (stt == 0) return;
            stt--;
            GanDuLieu(stt);
        }

        private void btnsau_Click(object sender, EventArgs e)
        {
            if (stt == tblSinhVien.Rows.Count - 1) return;
            stt++;
            GanDuLieu(stt);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}