namespace FeedReader.ApiControllers
{
    using System.Web.Http;

    using FeedReader.Class;

    using Model.Api.Interface;
    using Model.Persistence.Interface;

    [RoutePrefix("api/registration")]
    public class RegistrationController : ApiController
    {
        #region Fields

        private readonly IRegistrationService registrationService;

        #endregion

        #region Constructors and Destructors

        public RegistrationController(IRegistrationService registrationService)
        {
            this.registrationService = registrationService;
        }

        #endregion

        #region Public Methods and Operators

        [Authorize]
        [HttpDelete]
        [Route("")]
        public void Delete()
        {
            var identity = (Identity)this.User.Identity;
            this.registrationService.Unregister(identity.User.GetUserGuid());
        }

        [HttpHead]
        [Route("")]
        public void Get(string userName)
        {
            this.registrationService.UserNameExists(userName);
        }

        [HttpPost]
        [Route("")]
        public IToken Post(RegisterDto registerDto)
        {
            IToken token;
            this.registrationService.Register(
                registerDto.UserName,
                registerDto.Password,
                registerDto.TokenName,
                out token);
            return token;
        }

        #endregion

        public class RegisterDto
        {
            #region Public Properties

            public string Password { get; set; }

            public string TokenName { get; set; }

            public string UserName { get; set; }

            #endregion
        }
    }
}