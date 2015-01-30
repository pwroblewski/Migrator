using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace Migrator.Helpers
{
    public static class CustomPaste
    {
        private static IList<DataGridCellInfo> cells;

        public static void Paste(DataGrid grid)
        {
            var ret = System.Windows.Forms.Clipboard.GetData(System.Windows.DataFormats.Text) as string;
            var rows = ret.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            var cols = rows[0].Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);

            string[,] matrix = new string[rows.Length, cols.Length];

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                var col = rows[i].Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);

                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    matrix[i, j] = col[j];
                }
            }

            cells = grid.SelectedCells;

            var colCount = cells.Select(x => x.Column.DisplayIndex).Distinct().Count();
            var rowCount = cells.Count / colCount;

            int count = 0;

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    var cellItem = cells[count].Item;

                    if (matrix.GetLength(0) > i && matrix.GetLength(1) > j)
                        cellItem.GetType().GetProperty(cells[count].Column.SortMemberPath).SetValue(cellItem, matrix[i, j], null);

                    count++;
                }
            }

            grid.Items.Refresh();
        }
    }
}
