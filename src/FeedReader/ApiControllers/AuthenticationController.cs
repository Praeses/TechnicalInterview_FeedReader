namespace FeedReader.ApiControllers
{
    using System.Web.Http;

    using FeedReader.Class;

    using Model.Api.Interface;
    using Model.Persistence.Interface;

    using Shared.Exceptions;

    [RoutePrefix("api/authentication")]
    public class AuthenticationController : ApiController
    {
        #region Fields

        private readonly IAuthenticationService authenticationService;

        #endregion

        #region Constructors and Destructors

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
        }

        #endregion

        #region Public Methods and Operators

        [Authorize]
        [HttpDelete]
        [Route("")]
        public void Delete()
        {
            var identity = (Identity)this.User.Identity;
            this.authenticationService.Logout(identity.Token.GetTokenGuid());
        }

        [HttpHead]
        [Route("")]
        public void Get()
        {
            if (string.IsNullOrEmpty(this.User.Identity.Name))
            {
                throw new UnauthorizedException();
            }
        }

        [HttpPost]
        [Route("")]
        public IToken Post(LoginDto loginDto)
        {
            IToken token;
            this.authenticationService.Login(loginDto.UserName, loginDto.Password, loginDto.ToString(), out token);
            return token;
        }

        #endregion

        public class LoginDto
        {
            #region Public Properties

            public string Password { get; set; }

            public string TokenName { get; set; }

            public string UserName { get; set; }

            #endregion
        }
    }
}