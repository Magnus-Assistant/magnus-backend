using magnus_backend.Enums;
using magnus_backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace magnus_backend.Interfaces;

public interface IMagnusLog
{
    public ActionResult Log(MagnusLogModel log);
}