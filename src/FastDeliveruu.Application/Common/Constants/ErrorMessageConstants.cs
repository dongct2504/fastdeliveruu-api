namespace FastDeliveruu.Application.Common.Constants;

public static class ErrorMessageConstants
{
    // cities
    public const string CityNotFound = "Không tìm thấy Thành Phố";
    public const string CityDuplicate = "Thành phố đã tồn tại";
    public const string CityCustomerDelete = "Không thể xóa Thành Phố vì thành phố này đang được dùng bởi một hoặc nhiều người dùng";
    public const string CityShipperDelete = "Không thể xóa Thành Phố vì thành phố này đang được dùng bởi một hoặc nhiều người giao hàng";
    public const string CityRestaurantDelete = "Không thể xóa Thành Phố vì thành phố này đang được dùng bởi một hoặc nhiều nhà hàng";
    public const string CityOrderDelete = "Không thể xóa Thành Phố vì thành phố này đang được dùng bởi một hoặc đơn hàng";

    // districts
    public const string DistrictNotFound = "Không tìm thấy Quận/Huyện";
    public const string DistrictDuplicate = "Quận/Huyện đã tồn tại";
    public const string DistrictCustomerDelete = "Không thể xóa Quận/Huyện vì quận/huyện này đang được dùng bởi một hoặc nhiều người dùng";
    public const string DistrictShipperDelete = "Không thể xóa Quận/Huyện vì quận/huyện này đang được dùng bởi một hoặc nhiều người giao hàng";
    public const string DistrictRestaurantDelete = "Không thể xóa Quận/Huyện vì quận/huyện này đang được dùng bởi một hoặc nhiều nhà hàng";
    public const string DistrictOrderDelete = "Không thể xóa Quận/Huyện vì quận/huyện này đang được dùng bởi một hoặc nhiều đơn hàng";

    // wards
    public const string WardNotFound = "Không tìm thấy Phường/Xã";
    public const string WardDuplicate = "Phường/Xã đã tồn tại";
    public const string WardCustomerDelete = "Không thể xóa Phường/Xã vì phường/xã này đang được dùng bởi một hoặc nhiều người dùng";
    public const string WardShipperDelete = "Không thể xóa Phường/Xã vì phường/xã này đang được dùng bởi một hoặc nhiều người giao hàng";
    public const string WardRestaurantDelete = "Không thể xóa Phường/Xã vì phường/xã này đang được dùng bởi một hoặc nhiều nhà hàng";
    public const string WardOrderDelete = "Không thể xóa Phường/Xã vì phường/xã này đang được dùng bởi một hoặc nhiều đơn hàng";

    // lat, long
    public const string LatLongNotFound = "Không tìm thấy Tọa Độ gần nhất, vui lòng kiểm tra lại Địa Chỉ Đường";

    // genres
    public const string GenreNotFound = "Không tìm thấy Thể Loại";
    public const string GenreDuplicate = "Thể loại đã tồn tại";

    // restaurants
    public const string RestaurantNotFound = "Không tìm thấy Nhà Hàng";
    public const string RestaurantDuplicate = "Nhà Hàng đã tồn tại";

    // restaurant hours
    public const string RestaurantHourNotFound = "Không tìm thấy Giờ Hoạt Động của Nhà Hàng";
    public const string RestaurantHourDuplicate = "Giờ Hoạt Động của Nhà Hàng đã tồn tại";

    // menu items
    public const string MenuItemNotFound = "Không tìm thấy Món Ăn";
    public const string MenuItemDuplicate = "Món Ăn đã tồn tại";

    // menu item inventory
    public const string MenuItemInventoryNotFound = "Không tìm thấy Kho Món Ăn";
    public const string MenuItemInventoryNotEnough = "Món ăn này không đủ hàng tồn kho";
    public const string MenuItemInventoryDuplicate = "Kho Món Ăn đã tồn tại";

    // menu variants
    public const string MenuVariantNotFound = "Không tìm thấy Loại Món Ăn";
    public const string MenuVariantDuplicate = "Loại của Món Ăn đã tồn tại";

    // menu variant inventory
    public const string MenuVariantInventoryNotFound = "Không tìm thấy Kho của Loại Món Ăn";
    public const string MenuVariantInventoryNotEnough = "Loại Món ăn này không đủ hàng tồn kho";
    public const string MenuVariantInventoryDuplicate = "Kho của Loại Món Ăn đã tồn tại";

    // customer carts
    public const string CustomerCartNotFound = "Không tìm thấy Giỏ Hàng của Khách Hàng";
    public const string CustomerCartEmpty = "Giỏ Hàng hiện đang trống";
    public const string CustomerCartDuplicate = "Giỏ Hàng của Khách Hàng đã tồn tại";

    // cart items
    public const string CartItemNotFound = "Không tìm thấy Mặt Hàng trong Giỏ Hàng";
    public const string CartItemDuplicate = "Mặt Hàng trong Giỏ Hàng đã tồn tại";

    // wishlists
    public const string WishListNotFound = "Không tìm thấy phần Yêu Thích của Khách Hàng";
    public const string WishListEmpty = "Phần Yêu Thích hiện đang trống";
    public const string WishListDuplicate = "Phần Yêu Thích của Khách Hàng đã tồn tại";

    // orders
    public const string OrderNotFound = "Không tìm thấy Đơn Hàng";
    public const string OrderDuplicate = "Đơn Hàng đã tồn tại";

    // payments
    public const string PaymentNotFound = "Không tìm thấy phiếu thanh toán";
    public const string PaymentDuplicate = "Phiếu thanh toán đã tồn tại";

    // app users
    public const string AppUserNotFound = "Không tìm thấy Người Dùng";
    public const string AppUserDuplicate = "Email hoặc UserName của người dùng đã tồn tại";

    // shippers
    public const string ShipperNotFound = "Không tìm thấy Người Giao Hàng";
    public const string ShipperDuplicate = "Người Giao Hàng đã tồn tại";

    // deliveries
    public const string DeliveryNotFound = "Không tìm thấy Phương thức thanh toán";
    public const string DeliveryDuplicate = "Phương thức thanh toán này đã tồn tại";
    public const string DeliveryValidator = "Phương thức thanh toán phải là tiền mặt (cash), VNPAY hoặc là PayPal";

    // authen, confirms
    public const string WrongUserName = "Tên đăng nhập không hợp lệ";
    public const string WrongPassword = "Mật khẩu không hợp lệ";
    public const string EmailYetConfirmed = "Chưa xác thực email";
    public const string OtpError = "Mã OTP không phù hợp hoặc đã hết hạn";
    public const string PhoneConfirmed = "Số điện thoại của tài khoản này đã xác thực";
    public const string PhoneYetConfirmed = "Số điện thoại của tài khoản này chưa xác thực";
    public const string PhoneDuplicated = "Số điện thoại đã tồn tại";
    public const string PhoneValidator = "Số điện thoại phải có +84 ở đầu và các số sau phải có độ dài từ 6-15 kí tự";

    // author
    public const string RoleValidator = "Vai trò của người dùng phải là Customer, Staff hoặc Admin";
}
