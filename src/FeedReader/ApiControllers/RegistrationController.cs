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
            this.registrationService.UnregisterUser(identity.User.GetUserGuid());
        }

        [HttpHead]
        [Route("")]
        public void Get(string userName)
        {
            this.registrationService.UserNameExists(userName);
        }

        [HttpPost]
        [Route("")]
        public IToken Post(RegisterUserDto registerUserDto)
        {
            IToken token;
            this.registrationService.RegisterUser(
                registerUserDto.UserName,
                registerUserDto.Password,
                registerUserDto.TokenName,
                out token);
            return token;
        }

        #endregion

        public class RegisterUserDto
        {
            #region Public Properties

            public string Password { get; set; }

            public string TokenName { get; set; }

            public string UserName { get; set; }

            #endregion
        }
    }
}