using System.Text;
using System.Collections.Generic;

namespace System
{
    public interface IMemberSecurity
    {
        MemberIdentity GetByUserId(string userId);

        MemberIdentity GetByToken(string token);

        bool Register(MemberIdentity memberIdentity, bool encoded = false);
        bool Register(string name, string key, out MemberIdentity di, string ip = "");
        bool Register(string token, out MemberIdentity di, string ip = "");

        bool VerifyIdentity(MemberIdentity member, string checkPasswd);

        bool VerifyToken(MemberIdentity member, string checkToken);

        string CreateToken(MemberIdentity member);
    }

}
