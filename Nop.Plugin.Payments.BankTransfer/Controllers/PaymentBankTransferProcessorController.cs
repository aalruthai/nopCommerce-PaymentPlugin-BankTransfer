using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Media;
using Nop.Core.Infrastructure;
using Nop.Plugin.Payments.BankTransfer.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Payments.BankTransfer.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class PaymentBankTransferProcessorController : BasePaymentController
    {
        #region Fields

        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IDownloadService _downloadService;
        private readonly INopFileProvider _fileProvider;

        #endregion

        #region Ctor

        public PaymentBankTransferProcessorController(ILanguageService languageService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IStoreContext storeContext,
            IDownloadService downloadService,
            INopFileProvider fileProvider)
        {
            _languageService = languageService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _storeContext = storeContext;
            _downloadService = downloadService;
            _fileProvider = fileProvider;
        }

        #endregion

        #region Methods
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public virtual async Task<IActionResult> UploadFileBankTransfer()
        {


            var httpPostedFile = Request.Form.Files.FirstOrDefault();
            if (httpPostedFile == null)
            {
                return Json(new
                {
                    success = false,
                    message = "No file uploaded",
                    downloadGuid = Guid.Empty
                });
            }
            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var bankTransferSettings = await _settingService.LoadSettingAsync<BankTransferSettings>(storeScope);

            var fileBinary = await _downloadService.GetDownloadBitsAsync(httpPostedFile);

            var qqFileNameParameter = "qqfilename";
            var fileName = httpPostedFile.FileName;
            if (string.IsNullOrEmpty(fileName) && Request.Form.ContainsKey(qqFileNameParameter))
                fileName = Request.Form[qqFileNameParameter].ToString();
            //remove path (passed in IE)
            fileName = _fileProvider.GetFileName(fileName);

            var contentType = httpPostedFile.ContentType;

            var fileExtension = _fileProvider.GetFileExtension(fileName);
            if (!string.IsNullOrEmpty(fileExtension))
                fileExtension = fileExtension.ToLowerInvariant();

            
                //compare in bytes
                var maxFileSizeBytes = bankTransferSettings.MaxFileSize * 1024;
                if (fileBinary.Length > maxFileSizeBytes)
                {
                    //when returning JSON the mime-type must be set to text/plain
                    //otherwise some browsers will pop-up a "Save As" dialog.
                    return Json(new
                    {
                        success = false,
                        message = string.Format(await _localizationService.GetResourceAsync("ShoppingCart.MaximumUploadedFileSize"), bankTransferSettings.MaxFileSize),
                        downloadGuid = Guid.Empty
                    });
                }
            

            var download = new Download
            {
                DownloadGuid = Guid.NewGuid(),
                UseDownloadUrl = false,
                DownloadUrl = string.Empty,
                DownloadBinary = fileBinary,
                ContentType = contentType,
                //we store filename without extension for downloads
                Filename = _fileProvider.GetFileNameWithoutExtension(fileName),
                Extension = fileExtension,
                IsNew = true
            };
            await _downloadService.InsertDownloadAsync(download);

            //when returning JSON the mime-type must be set to text/plain
            //otherwise some browsers will pop-up a "Save As" dialog.
            return Json(new
            {
                success = true,
                message = await _localizationService.GetResourceAsync("ShoppingCart.FileUploaded"),
                downloadUrl = Url.Action("GetFileUpload", "Download", new { downloadId = download.DownloadGuid }),
                downloadGuid = download.DownloadGuid,
                downloadId = download.Id
            });
        }

        [HttpDelete]
        [IgnoreAntiforgeryToken]
        public virtual async Task<IActionResult> UploadFileBankTransfer(int id, Guid downid)
        {


            if (id <= 0 || downid == Guid.Empty)
            {
                return BadRequest();
            }



            var download = await _downloadService.GetDownloadByGuidAsync(downid);
            if (download == null || download.Id != id)
            {
                return BadRequest();
            }
            await _downloadService.DeleteDownloadAsync(download);

            //when returning JSON the mime-type must be set to text/plain
            //otherwise some browsers will pop-up a "Save As" dialog.
            return Ok();
        }
        #endregion
    }
}
