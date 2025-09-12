using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVOil.Data.Repositories
{
    public class ForecastRepository
    {


        //Calculate product info -> call store procedure

        //filter data and insert to temp table - option 1
            //calculate data by command, product, warehouse, Hour
            // sum volume by product, warehouse, Hour
            // avg volume by Hour group by product, warehouse
            // để tính cả loại hàng không lẫn và từng trường hợp lẫn thì dùng outter join để nối 3 lần select với nhau.


        //Get last avg volume by Hour -> store này xử lý tất cả việc lọc và tính toán dữ liệu bên trên luôn.
            //Dùng 1 bảng tạm để tính dữ liệu trung bình của các trường hợp trên.
            //sau đó dùng store để gọi ra.

        //Xóa dữ liệu cũ và cập nhật dữ liệu dự báo đã tính toán theo danh sách.

        // Get estimate ammont 



    }
}
