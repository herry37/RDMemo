namespace TestWebApl.Entitie
{
    /// <summary>
    /// 產品實體
    /// </summary>
    public class Product 
    {
        /// <summary>
        /// 產品編號
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 產品名稱
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 產品數量
        /// </summary>
        public int? Quantity { get; set; }
    }
}
