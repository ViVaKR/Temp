using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace VivForms
{
    public partial class CustomComboBox : Form
    {
        /// <summary>
        /// 드롭다운 리스트박스 토글용
        /// </summary>
        private bool isActivated;

        public bool IsActivated
        {
            get => isActivated;
            set
            {

                dropDown.Visible = value;
                isActivated = value;
            }
        }

        private readonly ComboBoxState comboBox;    // 콤보박스 스타일링
        private Rectangle comboText;                // 콤보박스 텍스트박스 사각모양
        private readonly ListBox dropDown;          // 드롭다운 메뉴


        public CustomComboBox()
        {
            InitializeComponent();

            // 폼 기본설정
            Width = 1024;
            Height = 768;
            StartPosition = FormStartPosition.CenterScreen;

            // 드롭다운 화살표 없는 콤보 만들기 샘플
            comboBox = new ComboBoxState();
            comboText = new Rectangle(new Point((Width - 300) / 2, 10), new Size(300, 30));

            // 드롭다운 메뉴 아이템 리스트 박스
            Controls.Add(dropDown = new ListBox
            {
                Top = 42,
                Left = (Width - 300) / 2,
                Size = new Size(300, 300),
                BackColor = Color.LightYellow

            });

            dropDown.Click += ListBox_Click;

            for (int i = 1; i < 60; i++)
            {
                dropDown.Items.Add($"{i:00}");
            }

            dropDown.BringToFront();

            IsActivated = false;
        }

        /// <summary>
        /// 드롭다운에서 아이템 선택시 이벤트 처리
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBox_Click(object sender, System.EventArgs e)
        {
            if (!(sender is ListBox control)) return;

            Invalidate();
            IsActivated = !IsActivated;

        }

        /// <summary>
        /// 콤보박스 그리기
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (!ComboBoxRenderer.IsSupported) return;

            ComboBoxRenderer.DrawTextBox(e.Graphics, comboText, dropDown.SelectedItem as string, Font, comboBox);
        }

        /// <summary>
        /// 콤보 텍스트 클릭시 드롭다운 메뉴 토글하기
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            // 콤보 텍스트 박스 클릭시 드롭다운 목록 토글하기(Show / Hide)
            if (comboText.Contains(e.Location) && ComboBoxRenderer.IsSupported)
            {
                IsActivated = !IsActivated;
                Invalidate();
            }
        }

    }
}
