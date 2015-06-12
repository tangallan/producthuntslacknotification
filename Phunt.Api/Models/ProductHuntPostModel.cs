using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phunt.Api.Models
{
    //public class ScreenshotUrl
    //{
    //    public string __invalid_name__300px { get; set; }
    //    public string __invalid_name__850px { get; set; }
    //}

    //public class ImageUrl
    //{
    //    public string __invalid_name__30px { get; set; }
    //    public string __invalid_name__32px { get; set; }
    //    public string __invalid_name__40px { get; set; }
    //    public string __invalid_name__44px { get; set; }
    //    public string __invalid_name__48px { get; set; }
    //    public string __invalid_name__50px { get; set; }
    //    public string __invalid_name__60px { get; set; }
    //    public string __invalid_name__64px { get; set; }
    //    public string __invalid_name__73px { get; set; }
    //    public string __invalid_name__80px { get; set; }
    //    public string __invalid_name__88px { get; set; }
    //    public string __invalid_name__96px { get; set; }
    //    public string __invalid_name__100px { get; set; }
    //    public string __invalid_name__110px { get; set; }
    //    public string __invalid_name__120px { get; set; }
    //    public string __invalid_name__132px { get; set; }
    //    public string __invalid_name__146px { get; set; }
    //    public string __invalid_name__160px { get; set; }
    //    public string __invalid_name__176px { get; set; }
    //    public string __invalid_name__220px { get; set; }
    //    public string __invalid_name__264px { get; set; }
    //    public string original { get; set; }
    //}

    public class User
    {
        public int id { get; set; }
        public string name { get; set; }
        public string headline { get; set; }
        public string created_at { get; set; }
        public string username { get; set; }
        public string website_url { get; set; }
        //public ImageUrl image_url { get; set; }
        public string profile_url { get; set; }
    }

    //public class ImageUrl2
    //{
    //    public string __invalid_name__30px { get; set; }
    //    public string __invalid_name__32px { get; set; }
    //    public string __invalid_name__40px { get; set; }
    //    public string __invalid_name__44px { get; set; }
    //    public string __invalid_name__48px { get; set; }
    //    public string __invalid_name__50px { get; set; }
    //    public string __invalid_name__60px { get; set; }
    //    public string __invalid_name__64px { get; set; }
    //    public string __invalid_name__73px { get; set; }
    //    public string __invalid_name__80px { get; set; }
    //    public string __invalid_name__88px { get; set; }
    //    public string __invalid_name__96px { get; set; }
    //    public string __invalid_name__100px { get; set; }
    //    public string __invalid_name__110px { get; set; }
    //    public string __invalid_name__120px { get; set; }
    //    public string __invalid_name__132px { get; set; }
    //    public string __invalid_name__146px { get; set; }
    //    public string __invalid_name__160px { get; set; }
    //    public string __invalid_name__176px { get; set; }
    //    public string __invalid_name__220px { get; set; }
    //    public string __invalid_name__264px { get; set; }
    //    public string original { get; set; }
    //}

    //public class Maker
    //{
    //    public int id { get; set; }
    //    public string name { get; set; }
    //    public string headline { get; set; }
    //    public string created_at { get; set; }
    //    public string username { get; set; }
    //    public string website_url { get; set; }
    //    public ImageUrl2 image_url { get; set; }
    //    public string profile_url { get; set; }
    //}

    //public class CurrentUser
    //{
    //    public bool voted_for_post { get; set; }
    //    public bool commented_on_post { get; set; }
    //}

    public class ProductHuntPost
    {
        public int id { get; set; }
        public string name { get; set; }
        public string tagline { get; set; }
        public string day { get; set; }
        public string created_at { get; set; }
        public bool featured { get; set; }
        public int comments_count { get; set; }
        public int votes_count { get; set; }
        public string discussion_url { get; set; }
        public string redirect_url { get; set; }
        //public ScreenshotUrl screenshot_url { get; set; }
        public User user { get; set; }
        //public List<Maker> makers { get; set; }
        //public CurrentUser current_user { get; set; }
        public bool maker_inside { get; set; }
        public object exclusive { get; set; }

        public DateTime created_at_datetime
        {
            get { return DateTime.Parse(created_at); }
        }
    }

    public class ProductHuntPostModel
    {
        public List<ProductHuntPost> posts { get; set; }
    }
}
