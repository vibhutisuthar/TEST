using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Recaptcha;
using System.Web.UI;
using System.IO;

namespace MTP_JAPA.Helpers
{
  public class CaptchaValidatorAttribute : ActionFilterAttribute
  {
    private const string CHALLENGE_FIELD_KEY = "recaptcha_challenge_field";
    private const string RESPONSE_FIELD_KEY = "recaptcha_response_field";

    public static string PRIVATE_KEY = "6LdlbNESAAAAAHX_HM8Ksi2AWt3kE60f0iHLTgIt";
    public static string PUBLIC_KEY = "6LdlbNESAAAAALtKoeah840s3cll8VsYjTBcUgmw";

    public override void OnActionExecuting(ActionExecutingContext filterContext)  
        {  
            var captchaChallengeValue = filterContext.HttpContext.Request.Form[CHALLENGE_FIELD_KEY];  
            var captchaResponseValue = filterContext.HttpContext.Request.Form[RESPONSE_FIELD_KEY];  
            var captchaValidtor = new Recaptcha.RecaptchaValidator  
                                      {
                                          PrivateKey = PRIVATE_KEY,  
                                          RemoteIP = filterContext.HttpContext.Request.UserHostAddress,  
                                          Challenge = captchaChallengeValue,  
                                          Response = captchaResponseValue  
                                      };  
  
            var recaptchaResponse = captchaValidtor.Validate();  
  
        // this will push the result value into a parameter in our Action  
            filterContext.ActionParameters["captchaValid"] = recaptchaResponse.IsValid;  
  
            base.OnActionExecuting(filterContext);  
        }
  }


  public static class CaptchaHelper
  {
    public static MvcHtmlString Generate(this HtmlHelper helper)
    {

      var captchaControl = new Recaptcha.RecaptchaControl
      {
        ID = "recaptcha",
        Theme = "clean",
        PublicKey = CaptchaValidatorAttribute.PUBLIC_KEY,
        PrivateKey = CaptchaValidatorAttribute.PRIVATE_KEY
      };


      var htmlWriter = new HtmlTextWriter(new StringWriter());
      captchaControl.RenderControl(htmlWriter);
      return new MvcHtmlString(htmlWriter.InnerWriter.ToString());
    }
  }
 


}


