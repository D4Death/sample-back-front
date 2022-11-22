using System;
namespace sample_ultilities
{
    public class Result
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public bool Success => Code == ResponseCode.OK;
        public bool Error => Code != ResponseCode.OK;

        public Result()
        {
            this.Code = ResponseCode.OK;
        }
    }

    public class Result<T> : Result
    {
        public T Data { get; set; }

        public Result() : base() { }
        public Result(T data)
        {
            this.Code = ResponseCode.OK;
            this.Data = data;
        }
    }
}
