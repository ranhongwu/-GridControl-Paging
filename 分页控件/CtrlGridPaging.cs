using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;

namespace UserControls.Data
{
    public partial class CtrlGridPaging : UserControl
    {
        #region 定义变量
        private int dataSourceRowsCount;
        #endregion

        #region 定义属性
        /// <summary>
        /// 绑定的表格控件
        /// </summary>
        public GridControl BindingGrid
        {
            get; set;
        }

        /// <summary>
        /// 和控件关联的表格
        /// </summary>
        public DataTable BindingDataTable
        {
            get; set;
        }

        /// <summary>
        /// 显示的表格
        /// </summary>
        public DataTable ShowingDataTable
        {
            get; set;
        }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages
        {
            get; set;
        }

        /// <summary>
        /// 当前页
        /// </summary>
        public int CurrentPage
        {
            get; set;
        }

        /// <summary>
        /// 每页条数
        /// </summary>
        public int RowsCount
        {
            get; set;
        }
        #endregion

        public CtrlGridPaging()
        {
            InitializeComponent();
        }

        //选择每页显示条数
        private void cbxRowCount_SelectedIndexChanged(object sender, EventArgs e)
        {
            RowsCount = Convert.ToInt32(cbxRowCount.Text);
            CurrentPage = 1;
            TotalPages = (int)Math.Ceiling((double)BindingDataTable.Rows.Count / RowsCount);
            ShowingDataTable.Rows.Clear();
            SetShowingDataTable();

            tbxCurrentPage.Text = CurrentPage.ToString();
            lbTotalPages.Text = TotalPages.ToString();
        }

        //手动输入显示页面编号
        private void tbxCurrentPage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                int page = Convert.ToInt32(tbxCurrentPage.Text);
                if (page < 1)
                {
                    CurrentPage = 1;
                    tbxCurrentPage.Text = "1";
                }
                else if (page > TotalPages)
                {
                    CurrentPage = TotalPages;
                    tbxCurrentPage.Text = CurrentPage.ToString();
                }
                else
                {
                    CurrentPage = page;
                    tbxCurrentPage.Text = CurrentPage.ToString();
                }
                SetShowingDataTable();
            }
        }


        //点击首页按钮
        private void lbFirstPage_Click(object sender, EventArgs e)
        {
            if (CurrentPage == 1)
                return;
            CurrentPage = 1;
            tbxCurrentPage.Text = CurrentPage.ToString();
            SetShowingDataTable();
        }

        //点击上一页按钮
        private void lbPrePage_Click(object sender, EventArgs e)
        {
            if (CurrentPage == 1)
                return;
            CurrentPage--;
            tbxCurrentPage.Text = CurrentPage.ToString();
            SetShowingDataTable();
        }

        //点击下一页按钮
        private void lbNextPage_Click(object sender, EventArgs e)
        {
            if (CurrentPage == TotalPages)
                return;
            CurrentPage++;
            tbxCurrentPage.Text = CurrentPage.ToString();
            SetShowingDataTable();
        }

        //末页按钮
        private void lbEnd_Click(object sender, EventArgs e)
        {
            if (CurrentPage == TotalPages)
                return;
            CurrentPage = TotalPages;
            tbxCurrentPage.Text = CurrentPage.ToString();
            SetShowingDataTable();
        }

        #region 封装方法
        /// <summary>
        /// 初始化控件显示
        /// </summary>
        /// <param name="dataTable"></param>
        public void InitCtrl(DataTable dataTable, GridControl gridControl)
        {
            try
            {
                #region 初始化属性
                BindingGrid = gridControl;
                BindingDataTable = dataTable;
                RowsCount = Convert.ToInt32(cbxRowCount.Text);
                dataSourceRowsCount = BindingDataTable.Rows.Count;
                TotalPages = (int)Math.Ceiling((double)dataSourceRowsCount / RowsCount);
                CurrentPage = dataSourceRowsCount == 0 ? 0 : 1;
                ShowingDataTable = new DataTable();
                if (BindingDataTable.Rows.Count == 0)
                {
                    #region 初始化控件显示
                    tbxCurrentPage.Text = CurrentPage.ToString();
                    lbTotalPages.Text = TotalPages.ToString();
                    lbTotalRows.Text = "共 " + dataSourceRowsCount.ToString() + " 条记录";
                    #endregion
                    return;
                }
                if (BindingDataTable.Rows.Count > 0 && BindingDataTable.Rows.Count <= RowsCount)
                {
                    ShowingDataTable = BindingDataTable.Copy();
                }
                else
                {
                    DataColumnCollection dataColumns = BindingDataTable.Columns;
                    foreach (DataColumn dataColumn in dataColumns)
                    {
                        ShowingDataTable.Columns.Add(dataColumn.ColumnName, dataColumn.DataType);
                    }
                    DataRow dataRow = null;
                    for (int i = 0; i < 20; i++)
                    {
                        dataRow = ShowingDataTable.NewRow();
                        int s = ShowingDataTable.Rows.Count;
                        dataRow = BindingDataTable.Rows[i];
                        ShowingDataTable.ImportRow(dataRow);
                    }
                }
                #endregion

                #region 初始化控件显示
                tbxCurrentPage.Text = CurrentPage.ToString();
                lbTotalPages.Text = "共 " + TotalPages.ToString() + " 页";
                lbTotalRows.Text = "共 " + dataSourceRowsCount.ToString() + " 条记录";
                #endregion
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("初始化列表失败");
                //添加日志记录相关方法
            }
            finally
            {
                BindingGrid.DataSource = ShowingDataTable;
                BindingGrid.Refresh();
            }
        }

        /// <summary>
        /// 根据当前页、显示行数确定显示的表格
        /// </summary>
        private void SetShowingDataTable()
        {
            try
            {
                if (BindingDataTable.Rows.Count <= RowsCount)
                {
                    ShowingDataTable = BindingDataTable.Copy();
                    return;
                }
                else
                {
                    int startRowIndex = (CurrentPage - 1) * RowsCount;
                    int endRoeIndex = startRowIndex + RowsCount;
                    if (endRoeIndex > BindingDataTable.Rows.Count)
                    {
                        endRoeIndex = BindingDataTable.Rows.Count;
                    }
                    ShowingDataTable.Rows.Clear();
                    DataRow dataRow = null;
                    for (int i = startRowIndex; i < endRoeIndex; i++)
                    {
                        dataRow = ShowingDataTable.NewRow();
                        dataRow = BindingDataTable.Rows[i];
                        ShowingDataTable.ImportRow(dataRow);
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("列表显示失败");
                //添加日志记录相关方法
            }
            finally
            {
                BindingGrid.DataSource = ShowingDataTable;
                BindingGrid.Refresh();
            }
        }
        #endregion

    }
}
