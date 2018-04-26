using System;

namespace P2P.Enroll
{
    enum MessageType : short
    {
        EnrollInit = 680,
        EnrollRegister = 681,
        EnrollSuccess = 682,
        EnrollFailure = 683
    }
}
