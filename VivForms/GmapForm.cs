using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Windows.Forms;

namespace VivForms
{
    public partial class GmapForm : Form
    {
        // GMap
        private readonly GMap.NET.WindowsForms.GMapControl gmap;

        // 레이어
        private readonly GMapOverlay markerOverlay;

        private double lat = 33.698292; // 기본 위도
        private double lng = 73.060766; // 기본 경도
        private bool tf = false;

        public GmapForm()
        {
            InitializeComponent();

            // 폼 기본설정 (전체화면)
            WindowState = FormWindowState.Maximized;
            DoubleBuffered = true;

            // GMap Instance & Default Settings
            gmap = new GMap.NET.WindowsForms.GMapControl
            {
                Visible = true,
                Dock = DockStyle.Fill,
                MinZoom = 5,
                MaxZoom = 100,
                Zoom = 13,
                CanDragMap = true,
                ShowCenter = true,
                DragButton = MouseButtons.Left,
                MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance,
                MouseWheelZoomType = MouseWheelZoomType.MousePositionAndCenter,
                ForceDoubleBuffer = true
            };

            gmap.MouseDown += Gmap_MouseDown;       // 좌클 마커 생성
            gmap.OnMarkerClick += (s, e) =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    markerOverlay.Markers.Remove(s);
                }
            }; // 우클 마커 지우기

            gmap.Manager.Mode = AccessMode.ServerAndCache;

            // GMap 폼에 넣기
            Controls.Add(gmap);

            // 다른 컨트롤에 의한 가려짐 방지
            gmap.BringToFront();

            // 마커 초기화
            markerOverlay = new GMapOverlay();
            gmap.Overlays.Add(markerOverlay);

            Load += (s, e) => // 폼 로드시 이벤트 구성
            {
                groupBox2.Width = 500; // 데이터 그리드뷰 폭설정
                SetGmap(); // 최초 좌표 구성하기
                GetData(); // 최초 데이터 (부표 1) 데이터 바인딩하기
            };

            // 부표 1 ~ 4 (Tag 를 통한 Access 데이터테이블 구분)
            Btn_Location_1.Tag = 1;
            Btn_Location_2.Tag = 2;
            Btn_Location_3.Tag = 3;
            Btn_Location_4.Tag = 4;
            Btn_Location_1.Click += SelectGroup;
            Btn_Location_2.Click += SelectGroup;
            Btn_Location_3.Click += SelectGroup;
            Btn_Location_4.Click += SelectGroup;

            Btn_Zoom_1.Click += (s, e) => { gmap.Zoom = 10; };
            Btn_Zoom_2.Click += (s, e) => { gmap.Zoom = 14; };
            Btn_Zoom_3.Click += (s, e) => { gmap.Zoom = 17; };

            // 데이터그리드뷰에서 행 바꿈선택 이벤트
            // 이때 지도를 갱신함
            dataGridView1.SelectionChanged += DataGridView1_SelectionChanged;

            groupBox2.Resize += (s, e) => { groupBox2.Width = 500; };
        }

        /// <summary>
        /// 지도 위에서 마우스 클릭시 해당 좌표의 
        /// 위도/경도 표시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gmap_MouseDown(object sender, MouseEventArgs e)
        {
            PointLatLng p = gmap.FromLocalToLatLng(e.X, e.Y);

            if (e.Button == MouseButtons.Left)
            {
                AddMarker(p, $"\nLat : {p.Lat}\nLng : {p.Lng}\n");
            }
        }

        /// <summary>
        /// 위/경도 표시
        /// </summary>
        /// <param name="p"></param>
        /// <param name="text"></param>
        public void AddMarker(PointLatLng p, string text)
        {
            GMarkerGoogle marker = new GMarkerGoogle(p, GMarkerGoogleType.blue_dot)
            {
                ToolTipMode = MarkerTooltipMode.OnMouseOver,
                ToolTipText = text

            };
            marker.ToolTip.Fill = new SolidBrush(Color.Tomato);
            marker.ToolTip.Foreground = new SolidBrush(Color.White);
            marker.ToolTip.Offset = new Point(-10, -30);
            marker.ToolTip.Stroke = new Pen(Color.Transparent, .0f);
            marker.ToolTip.Font = new Font("Tahoma", 13, FontStyle.Regular);
            marker.ToolTip.TextPadding = new Size(10, 10);
            markerOverlay.Markers.Add(marker);
        }

        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (!(sender is DataGridView view)) return;
            int idx = view.CurrentCell.RowIndex;

            if (idx == -1) return;


            // 위도
            tf = double.TryParse(view.Rows[idx].Cells[2].Value.ToString(), out lat);
            // 경도
            tf = double.TryParse(view.Rows[idx].Cells[3].Value.ToString(), out lng);
            if (!tf) return;
            SetGmap();
        }

        private void SetGmap()
        {
            gmap.Position = new PointLatLng(lat, lng);
        }

        private const string connStr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=data.accdb";

        private void Btn_Connect_Click(object sender, EventArgs e) => GetData();


        private void GetData(int tag = 1)
        {

            string strSQL = $"SELECT * FROM 부표{tag}";

            using (OleDbConnection connection = new OleDbConnection(connStr))
            {
                OleDbCommand command = new OleDbCommand(strSQL, connection);

                try
                {
                    connection.Open();

                    OleDbDataAdapter da = new OleDbDataAdapter(command);

                    DataTable table = new DataTable();
                    da.Fill(table);

                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = table;
                    dataGridView1.Refresh();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        /// 선택한 위도/경도에 따른 지도 표현
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectGroup(object sender, EventArgs e) => GetData(Convert.ToInt32((sender as Button).Tag));

    }
}
