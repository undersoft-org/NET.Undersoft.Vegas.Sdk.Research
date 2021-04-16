using System.Text;
using System.Collections.Generic;

namespace System
{
    public abstract class MemberSecurity : IMemberSecurity
    {
        public abstract MemberIdentity GetByUserId(string userId);

        public abstract MemberIdentity GetByToken(string token);

        public abstract bool Register(MemberIdentity memberIdentity, bool encoded = false);
        public abstract bool Register(string name, string key, out MemberIdentity di, string ip = "");
        public abstract bool Register(string token, out MemberIdentity di, string ip = "");

        public virtual bool VerifyIdentity(MemberIdentity member, string checkPasswd)
        {
            bool verify = false;

            string hashpasswd = member.Key;
            string saltpasswd = member.Salt;
            verify = CryptoHashkey.Verify(hashpasswd, saltpasswd, checkPasswd);

            return verify;
        }

        public virtual bool VerifyToken(MemberIdentity member, string checkToken)
        {
            bool verify = false;

            string token = member.Token;

            if (checkToken.Equals(token))
            {
                DateTime time = DateTime.Now;
                DateTime registerTime = member.RegisterTime;
                DateTime lastAction = member.LastAction;
                DateTime lifeTime = member.LifeTime;
                if (lifeTime > time)
                    verify = true;
                else if (lastAction > time.AddMinutes(-30))
                {
                    member.LifeTime = time.AddMinutes(30);
                    member.LastAction = time;
                    verify = true;
                }
            }
            return verify;
        }

        public virtual string CreateToken(MemberIdentity member)
        {
            string token = null;
            string key = member.Key;
            string timesalt = Convert.ToBase64String(DateTime.Now.Ticks.ToString().ToBytes(CharEncoding.ASCII));
            token = CryptoHashkey.Encrypt(key, 1, timesalt);
            member.Token = token;
            DateTime time = DateTime.Now;
            member.RegisterTime = time;
            member.LifeTime = time.AddMinutes(30);
            member.LastAction = time;
            return token;
        }
    }

}
