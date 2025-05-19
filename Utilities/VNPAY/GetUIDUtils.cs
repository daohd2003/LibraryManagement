using System.Text.RegularExpressions;

namespace LibraryManagement.Utilities.VNPAY
{
    public static class GetUIDUtils
    {
        /// <summary>
        /// Trích xuất userId từ OrderDescription có định dạng "UID:{userId}-description"
        /// </summary>
        /// <param name="orderDescription">Chuỗi mô tả từ VNPay (vnp_OrderInfo)</param>
        /// <returns>UserId nếu hợp lệ, ngược lại trả về 0</returns>
        public static int ExtractUserId(string orderDescription)
        {
            if (string.IsNullOrWhiteSpace(orderDescription))
                return 0;

            try
            {
                // Sử dụng Regex để tìm UID
                var match = Regex.Match(orderDescription, @"UID:(\d+)");
                if (match.Success && int.TryParse(match.Groups[1].Value, out int userId))
                {
                    return userId;
                }
            }
            catch
            {
                // ignored
            }

            return 0;
        }
    }
}
