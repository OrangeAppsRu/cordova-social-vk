using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using VK.WindowsPhone.SDK.Util;
using VK.WindowsPhone.SDK.API;
using VK.WindowsPhone.SDK.API.Model;

namespace VK.WindowsPhone.SDK.Pages
{
    public partial class VKPublishPage
    {
        enum UploadingStatus
        {
            NotStarted,
            InProgress,
            Failed,
            Completed
        };
        
        static internal readonly string INPUT_PARAM_ID = "VKPublishPageInputParams";

        private bool _isInitialized;

        private VKPublishInputData _inputData;

        private bool _isPublishing;

        private UploadingStatus _uploadingStatus;

        private VKPhoto _uploadedPhoto;

        public VKPublishPage()
        {
            InitializeComponent();


            PrivacyIcon.Source = new BitmapImage(new Uri(String.Format("Assets/sdk-privacy{0}-WVGA.png", true ? "" : "-set"), UriKind.Relative));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (!_isInitialized)
            {
                var publishParams = VKParametersRepository.GetParameterForIdAndReset(INPUT_PARAM_ID) as VKPublishInputData;


                _inputData = publishParams;

                InitializeUIFromInputParams();

                UpdateState();

                _isInitialized = true;
            }
        }

        private void InitializeUIFromInputParams()
        {
            PostTextBox.Text = _inputData.Text ?? "";

            if (_inputData.Image != null)
            {
                var bi = new BitmapImage() { CreateOptions = BitmapCreateOptions.None };
                bi.SetSource(_inputData.Image);
                PreviewImage.Source = bi;

                _inputData.Image.Position = 0;

                _uploadingStatus = UploadingStatus.NotStarted;

                UploadImage();
            }
            else
            {
                _uploadingStatus = UploadingStatus.Completed;
            }

            if (_inputData.ExternalLink != null)
            {

                LinkDescription.Text = _inputData.ExternalLink.Title ?? "";

                LinkDescription.Visibility = string.IsNullOrEmpty(LinkDescription.Text) ? Visibility.Collapsed : Visibility.Visible;

                if (!string.IsNullOrWhiteSpace(_inputData.ExternalLink.Title) && !string.IsNullOrWhiteSpace(_inputData.ExternalLink.Subtitle))
                {
                    LinkDomain.Text = _inputData.ExternalLink.Subtitle;
                }

                LinkDomain.Visibility = string.IsNullOrEmpty(LinkDomain.Text) ? Visibility.Collapsed : Visibility.Visible;
            }

        }

        private void PrivacyButtonClicked(object sender, GestureEventArgs e)
        {
            HiddenCheckBoxes.Visibility = HiddenCheckBoxes.Visibility == Visibility.Collapsed ? Visibility : Visibility.Collapsed;
        }

        private void FriendsOnlyCheckBox_OnChecked(object sender, RoutedEventArgs e)
        {
            if (PrivacyIcon != null)
                PrivacyIcon.Source = new BitmapImage(new Uri("Assets/sdk-privacy-set-WVGA.png", UriKind.Relative));

            UpdateTextBlockVisibility();
        }        

        private void FriendsOnlyCheckBox_OnUnchecked(object sender, RoutedEventArgs e)
        {
            if (PrivacyIcon != null)
                PrivacyIcon.Source = new BitmapImage(new Uri("Assets/sdk-privacy-WVGA.png", UriKind.Relative));
            
            UpdateTextBlockVisibility();
        }

        private void UpdateTextBlockVisibility()
        {
            textBlockEveryone.Visibility = FriendsOnlyCheckBox.IsChecked.HasValue && FriendsOnlyCheckBox.IsChecked.Value ? Visibility.Collapsed : Visibility.Visible;
            textBlockFriendsOnly.Visibility = textBlockEveryone.Visibility == System.Windows.Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        private void buttonPublish_Click(object sender, RoutedEventArgs e)
        {
            Publish();
        }

        private void UploadImage()
        {
            if (_uploadingStatus == UploadingStatus.Completed || _uploadingStatus == UploadingStatus.InProgress)
            {
                return;
            }

            _uploadingStatus = UploadingStatus.InProgress;

            UpdateState();

            VKUploadRequest.CreatePhotoWallUploadRequest().Dispatch(
                _inputData.Image,
                (progress) =>
                {
                    VKExecute.ExecuteOnUIThread(() =>
                        {
                            ProgressUpload.Value = progress;
                        });
                },
                (res) =>
                {
                    VKExecute.ExecuteOnUIThread(() =>
                        {
                            if (res.ResultCode == VKResultCode.Succeeded)
                            {
                                _uploadingStatus = UploadingStatus.Completed;

                                _uploadedPhoto = res.Data;


                            }
                            else
                            {
                                _uploadingStatus = UploadingStatus.Failed;
                            }
                            UpdateState();
                            
                        });
                });
        
        }

        private void Publish()
        {
            if (_isPublishing)
            {
                return;
            }

            _isPublishing = true;
            UpdateState();
            var parameters =  PrepareParameters();

            var request = new VKRequest(parameters);

            request.Dispatch<object>(
                (res) =>
                {
                    if (res.ResultCode == VKResultCode.Succeeded)
                    {
                        VKExecute.ExecuteOnUIThread(()=>
                            {
                                NavigationService.GoBack();
                                _isPublishing = false;
                                UpdateState();
                            });
                    }
                    else
                    { 
                          VKExecute.ExecuteOnUIThread(() =>
                                {
                                    MessageBox.Show(Localization.Resources.Error);
                                    _isPublishing = false;
                                    UpdateState();
                                });
                    }
                });
        }

        private void UpdateState()
        {
            progressBar.Visibility = _isPublishing ? Visibility.Visible : Visibility.Collapsed;

            ContentPanel.IsHitTestVisible = _isPublishing ? false : true;

            switch (_uploadingStatus)
            {
                case UploadingStatus.Completed:
                    buttonPublish.IsHitTestVisible = true;
                    ProgressUpload.Visibility = Visibility.Collapsed;
                    PreviewImage.Opacity = 1;
                    textBlockFailed.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case UploadingStatus.Failed:
                    buttonPublish.IsHitTestVisible = false;
                    ProgressUpload.Visibility = Visibility.Collapsed;
                    textBlockFailed.Visibility = System.Windows.Visibility.Visible;
                    PreviewImage.Opacity = 0.5;
                    break;
                case UploadingStatus.NotStarted:
                    buttonPublish.IsHitTestVisible = false;
                    ProgressUpload.Visibility = Visibility.Collapsed;
                    PreviewImage.Opacity = 0.5;
                    textBlockFailed.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case UploadingStatus.InProgress:
                    buttonPublish.IsHitTestVisible = false;
                    ProgressUpload.Visibility = Visibility.Visible;
                    PreviewImage.Opacity = 0.5;
                    textBlockFailed.Visibility = System.Windows.Visibility.Collapsed;
                    break;

            }
        }

        private VKRequestParameters PrepareParameters()
        {
            var parameters = new Dictionary<string, string>();

            parameters["message"] = PostTextBox.Text;

            var listAttachments = new List<string>();

            if (_uploadedPhoto != null)
            {
                listAttachments.Add("photo" + _uploadedPhoto.owner_id + "_" + _uploadedPhoto.id);
            }

            if (_inputData.ExternalLink != null && !string.IsNullOrWhiteSpace(_inputData.ExternalLink.Uri))
            {

                listAttachments.Add(_inputData.ExternalLink.Uri);
                
            }

            if (listAttachments.Count > 0)
            {
                var attachmentsStr = StrUtil.GetCommaSeparated(listAttachments);
                parameters.Add("attachments", attachmentsStr );
            }

            bool exportToTwitter = TwitterExportCheckBox.IsChecked.HasValue && TwitterExportCheckBox.IsChecked.Value;
            if (exportToTwitter)
            {
                parameters["services"] = "twitter";
            }

            if (FacebookExportCheckBox.IsChecked.HasValue && FacebookExportCheckBox.IsChecked.Value)
            {
                if (exportToTwitter)
                {
                    parameters["services"] = "twitter,facebook";
                }
                else
                {
                    parameters["services"] = "facebook";
                }
            }

            if (FriendsOnlyCheckBox.IsChecked.HasValue && FriendsOnlyCheckBox.IsChecked.Value)
            {
                parameters["friends_only"] = "1";
            }

            return new VKRequestParameters("wall.post", parameters);
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void Grid_Tap(object sender, GestureEventArgs e)
        {
            if (_uploadingStatus == UploadingStatus.Failed)
            {
                UploadImage();
            }
        }
    }
}