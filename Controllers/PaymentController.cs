using LibraryManagement.Data;
using LibraryManagement.DTOs.Request;
using LibraryManagement.Enums.VNPay;
using LibraryManagement.Models.VNPay;
using LibraryManagement.Services.Payments.QRCodeServices;
using LibraryManagement.Services.Payments.VNPay;
using LibraryManagement.Utilities.VNPAY;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace LibraryManagement.Controllers
{
    [ApiController]
    [Route("api/payment/Vnpay")]
    public class VNPayController : ControllerBase
    {
        private readonly IVnpay _vnpay;
        private readonly IConfiguration _configuration;
        private readonly ILogger<VNPayController> _logger;

        public VNPayController(IVnpay vnpay, IConfiguration configuration, ILogger<VNPayController> logger)
        {
            _vnpay = vnpay;
            _configuration = configuration;

            _vnpay.Initialize(_configuration["Vnpay:TmnCode"], _configuration["Vnpay:HashSecret"], _configuration["Vnpay:BaseUrl"], _configuration["Vnpay:CallbackUrl"]);
            _logger = logger;
        }

        /// <summary>
        /// Tạo url thanh toán
        /// </summary>
        /// <param name="money">Số tiền phải thanh toán</param>
        /// <param name="description">Mô tả giao dịch</param>
        /// <returns></returns>
        [HttpGet("CreatePaymentUrl")]
        public ActionResult<string> CreatePaymentUrl(double money, string description)
        {
            try
            {
                var ipAddress = NetworkHelper.GetIpAddress(HttpContext); // Lấy địa chỉ IP của thiết bị thực hiện giao dịch

                var request = new PaymentRequest
                {
                    PaymentId = DateTime.Now.Ticks,
                    Money = money,
                    Description = description,
                    IpAddress = ipAddress,
                    BankCode = BankCode.ANY, // Tùy chọn. Mặc định là tất cả phương thức giao dịch
                    CreatedDate = DateTime.Now, // Tùy chọn. Mặc định là thời điểm hiện tại
                    Currency = Currency.VND, // Tùy chọn. Mặc định là VND (Việt Nam đồng)
                    Language = DisplayLanguage.Vietnamese // Tùy chọn. Mặc định là tiếng Việt
                };

                var paymentUrl = _vnpay.GetPaymentUrl(request);

                return Created(paymentUrl, paymentUrl);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Thực hiện hành động sau khi thanh toán. URL này cần được khai báo với VNPAY để API này hoạt đồng (ví dụ: http://localhost:1234/api/Vnpay/IpnAction)
        /// </summary>
        /// <returns></returns>
        [HttpGet("IpnAction")]
        [HttpPost("IpnAction")]
        public IActionResult IpnAction()
        {
            _logger.LogInformation("IpnAction endpoint was called at {Time}", DateTime.Now);

            if (Request.QueryString.HasValue)
            {
                try
                {
                    var paymentResult = _vnpay.GetPaymentResult(Request.Query);
                    if (paymentResult.IsSuccess)
                    {
                        _logger.LogInformation("Payment success for PaymentId: {PaymentId}", paymentResult.PaymentId);
                        // Thực hiện hành động nếu thanh toán thành công tại đây. Ví dụ: Cập nhật trạng thái đơn hàng trong cơ sở dữ liệu.
                        return Ok();
                    }
                    _logger.LogWarning("Payment failed for PaymentId: {PaymentId}", paymentResult.PaymentId);
                    // Thực hiện hành động nếu thanh toán thất bại tại đây. Ví dụ: Hủy đơn hàng.
                    return BadRequest("Thanh toán thất bại");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            _logger.LogWarning("IpnAction called but query string is empty");
            return NotFound("Không tìm thấy thông tin thanh toán.");
        }

        /// <summary>
        /// Trả kết quả thanh toán về cho người dùng
        /// </summary>
        /// <returns></returns>
        [HttpGet("Callback")]
        public ActionResult<PaymentResult> Callback()
        {
            _logger.LogInformation("Callback endpoint was called at {Time}", DateTime.Now);

            if (Request.QueryString.HasValue)
            {
                try
                {
                    var paymentResult = _vnpay.GetPaymentResult(Request.Query);

                    if (paymentResult.IsSuccess)
                    {
                        _logger.LogInformation("Payment success for PaymentId: {PaymentId}", paymentResult.PaymentId);
                        return Ok(paymentResult);
                    }
                    _logger.LogWarning("Payment failed for PaymentId: {PaymentId}", paymentResult.PaymentId);
                    return BadRequest(paymentResult);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing Callback");
                    return BadRequest(ex.Message);
                }
            }
            _logger.LogWarning("Callback called but query string is empty");
            return NotFound("Không tìm thấy thông tin thanh toán.");
        }
    }
}
