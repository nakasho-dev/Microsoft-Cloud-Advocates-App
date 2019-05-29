﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Advocates.Models;
using Advocates.Services;
using Microsoft.AppCenter.Data;
using MvvmHelpers;
using Prism.Commands;
using Prism.Navigation;
using Xamarin.Forms;

namespace Advocates.ViewModels
{
    public class RssFeedPageViewModel : ViewModelBase
    {

        BlogPost selectedBlogPost;
        public BlogPost SelectedBlogPost
        {
            get => selectedBlogPost;
            set
            {
                SetProperty(ref selectedBlogPost, value);
                Xamarin.Essentials.Browser.OpenAsync(value.Url);
            }
        }

        int columnCount;
        public int ColumnCount
        {
            get => columnCount;
            set
            {
                SetProperty(ref columnCount, value);
            }
        }

        public ObservableRangeCollection<BlogPost> Filtered
        {
            get => filtered;
            set => SetProperty(ref filtered, value);
        }

        public bool IsRefreshing
        {
            get => isRefreshing;
            set => SetProperty(ref isRefreshing, value);
        }

        public string SearchText
        {
            get => searchText;
            set => SetProperty(ref searchText, value);
        }


        //Commands 
        public DelegateCommand BlogPostSelectedCommand { get; set; }
        public DelegateCommand RefreshCommand { get; set; }
        public DelegateCommand SearchCommand { get; set; }


        public RssFeedPageViewModel()
        {
            switch(Device.Idiom)
            {
                case TargetIdiom.Phone:
                    columnCount = 1;
                    break;
                case TargetIdiom.Tablet:
                    ColumnCount = 4;
                    break;
                case TargetIdiom.Desktop:
                    ColumnCount = 4;
                    break;
                case TargetIdiom.TV:
                    ColumnCount = 8;
                    break;
                case TargetIdiom.Watch:
                    ColumnCount = 1;
                    break;
                case TargetIdiom.Unsupported:
                    ColumnCount = 1;
                    break;
                default:
                    ColumnCount = 1;
                    break;
            }

        }

        public override async void OnNavigatingTo(INavigationParameters parameters)
        {
            await RefreshData(true, true);

        }


        void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
   
        }

        private async void BlogPostSelected(BlogPost selectedBlogPost)
        {
            await Xamarin.Essentials.Browser.OpenAsync(selectedBlogPost.Url);
        }

        private void Search()
        {
            if (string.IsNullOrEmpty(searchText))
            {
                //Show all items 
                Filtered.ReplaceRange(blogPosts);
            }
            else
            {
                var match = blogPosts.Where(x => x.Title.Contains(searchText));
                Filtered.ReplaceRange(match);
            }
        }

        private async void Refresh()
        {
            await RefreshData(false, true);
        }

        private async Task RefreshData(bool backgroundRefresh, bool force)
        {
            IsRefreshing = !backgroundRefresh;
            var bps = await Data.ListAsync<BlogPost>("readonly");
            var unsorted = bps.CurrentPage.Items.Select(a => a.DeserializedValue).Where(x => x.ClassType == "BlogPost");
            blogPosts.ReplaceRange(unsorted);
            Filtered = new ObservableRangeCollection<BlogPost>(blogPosts);
            IsRefreshing = false;
        }



        private ObservableRangeCollection<BlogPost> blogPosts = new ObservableRangeCollection<BlogPost>();
        private ObservableRangeCollection<BlogPost> filtered = new ObservableRangeCollection<BlogPost>();

        private readonly INavigationService navigationService;
        bool isRefreshing;
        string searchText;
    }
}
