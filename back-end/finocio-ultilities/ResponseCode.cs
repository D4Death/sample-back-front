using System;
namespace sample_ultilities
{
    public class ResponseCode
    {
        /// <summary>
        /// Thành công
        /// </summary>
        public const int OK = 200;

        /// <summary>
        /// Xuất hiện lỗi trong logic xử lý của service
        /// </summary>
        public const int SYS_GENERIC_ERROR = 1;

        /// <summary>
        /// Request không được phản hồi lại sau thời gian expected
        /// </summary>
        public const int TIMEOUT = 2;

        /// <summary>
        /// Tham số truyền vào cho service không hợp lệ
        /// </summary>
        public const int INVALID_PARAMETER = 3;

        /// <summary>
        /// Không có đủ quyền gọi đến function trong service
        /// </summary>
        public const int UNAUTHORIZED = 4;

        /// <summary>
        /// Token expired
        /// </summary>
        public const int TOKEN_INVALID = 5;

        /// <summary>
        /// Token expired
        /// </summary>
        public const int TOKEN_EXPIRED = 6;

        /// <summary>
        /// Record not found
        /// </summary>
        public const int RECORD_NOT_FOUND = 7;

        /// <summary>
        /// Duplicate entry
        /// </summary>
        public const int DUPLICATE_ENTRY = 8;

        /// <summary>
        /// Dữ liệu hệ thống không chính xác, không thể xử lý tiếp
        /// </summary>
        public const int INVALID_DATA = 9;

        /// <summary>
        /// Dữ liệu hệ thống không cho phép thực hiện yêu cầu
        /// </summary>
        public const int REQUEST_NOT_ALLOWED = 10;

        /// <summary>
        /// Dữ liệu tạm khóa
        /// </summary>
        public const int LOCK_DATA = 11;

        /// <summary>
        /// Giới hạn dữ liệu
        /// </summary>
        public const int LIMIT_ACCESS = 12;

        public static bool IsOk(int code)
        {
            return OK == code;
        }

        public static bool IsNotOk(int code)
        {
            return OK != code;
        }
    }
}
