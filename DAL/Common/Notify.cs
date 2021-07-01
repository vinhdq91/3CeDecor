namespace DAL.Common 
{ 
    public class Notify
    {
        public const string Notifys = "Notice";
        public const string AddSuccess = "Data created successfully"; //Dữ liệu được thêm mới thành công
        public const string AddError = "Data was not successfully created"; //Thêm mới dữ liệu không thành công
        public const string UpdateSuccess = "Data updated successfully"; //Dữ liệu được cập nhật thành công
        public const string ProfileUpdateSuccess = "Information updated successfully"; //Thông tin cá nhân cập nhật thành công
        public const string PhoneExist = "Phone number is already in use";//Phone đã được sử dụng
        public const string UpdateError = "Data was not successfully updated";
        public const string DeleteSuccess = "Data deleted successfully";
        public const string DeleteError = "Data was not successfully deleted";
        public const string DeleteErrorByUsing = "Data was not deleted because it's currently in use"; //Dữ liệu không thể xóa bởi vì đang được sử dụng
        public const string LoginSuccess = "Login successful";
        public const string LoginFail = "Login failed. Please try again";
        public const string LoginInfoIncorrect = "Email or password incorrect";
        public const string IncorrectPassword = "Password is not correct";
        public const string IncorrectCurrentPassword = "The current password you have entered is incorrect";
        public const string ChangePasswordSuccess = "Password changed successfully";
        public const string LoginNotActivated = "Account has not been activated. Please check your email inbox to activate your account";
        public const string LoginCodeIncorrect = "Account information is incorrect or has not been activated"; //Tài khoản khách hàng không chính xác Hoặc chưa được kích hoạt.
        public const string RegisterSuccess = "Account registration is successful. Please check your email inbox to activate your account"; //Đăng ký tài khoản thành công Kiếm tra hòm thư và kích hoạt tài khoản
        public const string RegisterFail = "Account registration failed";
        public const string ActivateAcount = "Activate account";
        public const string RecoverPassWord = "Password reset request";
        public const string ActivateFail = "Account was not successfully activated";
        public const string ActivateSuccess = "Account successfully activated";
        public const string AccountExist = "Email/phone number is already in use";//Email/phone đã được sử dụng
        public const string EmailExist = "Email is already in use";//Email đã được sử dụng 
        public const string AccountNotExist = "Account does not exist"; //Tài khoản không tồn tại
        public const string CookiesNotSupport = "Your browser does not support cookies";//Trình duyệt không hỗ trợ cookie
        public const string ExistsError = "An error occured during the update operation"; //có lỗi xảy ra trong quá trình cập nhật
        public const string EmailIncorrect = "Email or password incorrect.";
        public const string CreatePasswordSuccess = "A password recovery link has been sent to your email. Please check your inbox to reset your password."; //Chúng tôi đã gửi link xác nhận khôi phục mật khẩu tới email của bạn. Vui lòng kiểm tra email
        public const string EmailOnlySocial = "The email address was already used to sign up via social network. Please log in with your Facebook or Google+ account.";
        public const string AccountLocked = "This account has been locked";
        public const string AccountAlreadyActive = "This account is already active";
        #region Message product
        public const string ProductPostSuccess = "Listing posted successfully";
        public const string ProductPostSuccessMessage = "Thank you for using our service. Your listing will be processed and you will be informed of the result via email within 24 hours.";//Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi! Post của bạn sẽ được xử lý và kết quả xử lý sẽ được gửi đến email của bạn trong vòng 24 giờ
        public const string ProductUpdateSuccessMessage = "Information updated successfully. Your change(s) will be processed within 24 hours.";//Cập nhật thành công, Thay đổi của bạn sẽ được xử lý trong vòng 24h
        public const string ProductUpdateStatusSuccess = "Status updated successfully.";
        public const string ProductDeleteSuccess = "Listing successfully removed.";
        public const string ProductDisApproveSuccess = "Deactivated successfully."; //Hạ tin thành công
        public const string ProductSoldSuccess = "Congratulations on your SOLD listing! The ad will now be removed from the system."; //Cập nhật Sold thành công
        #endregion
        #region Message specificAdmin
        public const string Ad_UpdateSpecSuccess = "Cập nhật thông tin specific thành công!";
        public const string Ad_UpdateSpecError = "Có lỗi khi cập nhật thông tin specfic. Liên hệ IT để được hỗ trợ!";
        public const string Ad_AddSpecSucess = "Thêm specific thành công!";
        public const string Ad_AddSpecError = "Có lỗi khi thêm specific. Liên hệ IT để được hỗ trợ!";
        #endregion
        #region Message chung
        public const string Ad_UpdateSuccess = "Cập nhật thành công!";
        public const string Ad_UpdateError = "Có lỗi khi cập nhật. Liên hệ IT để được hỗ trợ!";
        public const string Ad_AddSucess = "Thêm thành công!";
        public const string Ad_AddError = "Có lỗi khi thêm. Liên hệ IT để được hỗ trợ!";
        public const string Ad_GetSucess = "Lấy dữ liệu thành công!!";
        public const string Ad_GetError = "Có lỗi khi lấy dữ liệu. Liên hệ IT để được hỗ trợ!";
        public const string Ad_UploadImageSuccess = "Tải ảnh lên thành công!";
        public const string Ad_UploadImageError = "Tải ảnh lên thất bại!";
        public const string Ad_UploadFormatImageError = "File được chọn phải là file ảnh!";
        public const string Ad_DuplicateEmail = "Email đã được sử dụng!";
        public const string Ad_DuplicatePhone = "Số điện thoại đã được sử dụng!";
        public const string Ad_DeleteUser_OverTime = "User được tạo quá 3h, không thể xóa!";
        public const string Ad_DeleteUser_HavedAction = "User đã có action, không thể xóa!";
        #endregion
        #region Message CreateAuto
        public const string CityValidate = "Please input your City";
        public const string BrandValidate = "Please input your Brand";
        public const string ModelValidate = "Please input your Model";
        public const string VersionValidate = "Please input your Version";
        public const string YearValidate = "Please input your Year";
        public const string AutoTypeValidate = "Please input your Body type";
        public const string TranmissionValidate = "Please input your Transmisson";
        public const string ColorValidate = "Please input your Color";
        public const string CaptchaCarValidate = "Please input your Verification";
        #endregion

        #region Message offer
        public const string AnOfferWasMade = "An offer was made for your car listing";
        public const string PriceDropExist = "You've already registered to get price drop notifications for this listing.";
        public const string PriceDropNoti = "Price drop notification";
        #endregion
        public const string SupportSendSuccess = "Your request has been sent";

        #region Message Comment
        public const string CommentSuccess = "Thank you for submitting your comment";
        public const string CommentFail = "Your comment has failed to submit. Please try again.";
        public const string CommentDelay = "Can act after 5s";
        public const string LikeSuccess = "Success";
        public const string LikeDelay = "Can act after 24h";
        #endregion
    }
}