using DemoApp.EmailHost.Data.Entity;
using DemoApp.EmailHost.Models;
using Microsoft.AspNetCore.Mvc;
using SharpDevLib.Standard;

namespace DemoApp.EmailHost.Controllers;

public class UserController(IServiceProvider serviceProvider) : BaseController(serviceProvider)
{
    public IActionResult Index()
    {
        var model = _userRepository.GetMany(x => !x.IsDeleted).Select(x => new UserViewModel { Id = x.Id, Name = x.Name, Password = x.Password }).ToList();
        return View(model);
    }

    public IActionResult Detail(Guid id)
    {
        var model = _userRepository.GetMany(x => x.Id == id).Select(x => new UserViewModel { Id = x.Id, Name = x.Name, Password = x.Password }).FirstOrDefault();
        return View(model);
    }

    public IActionResult Add(UserViewModel model)
    {
        return View(model);
    }

    [HttpPost]
    public IActionResult AddPost([Bind("Id,Name,Password")] UserViewModel model)
    {
        if (model.Name.IsNullOrWhiteSpace())
        {
            model.Error = "用户名是必需的";
            return View(nameof(Add), model);
        }

        if (model.Password.IsNullOrWhiteSpace())
        {
            model.Error = "密码是必需的";
            return View(nameof(Add), model);
        }

        var entity = new UserEntity(model.Name, model.Password);
        _userRepository.Add(entity);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Update(Guid id)
    {
        var model = _userRepository.GetMany(x => x.Id == id).Select(x => new UserViewModel { Id = x.Id, Name = x.Name, Password = x.Password }).FirstOrDefault();
        return View(model);
    }

    [HttpPost]
    public IActionResult UpdatePost(Guid id, [Bind("Id,Name,Password")] UserViewModel model)
    {
        if (model.Name.IsNullOrWhiteSpace())
        {
            model.Error = "用户名是必需的";
            return View(nameof(Update), model);
        }

        if (model.Password.IsNullOrWhiteSpace())
        {
            model.Error = "密码是必需的";
            return View(nameof(Update), model);
        }

        var entity = _userRepository.Get(x => x.Id == id) ?? throw new NullReferenceException();
        entity.Name = model.Name;
        entity.Password = model.Password;
        _userRepository.Update(entity);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(Guid id)
    {
        var model = _userRepository.GetMany(x => x.Id == id).Select(x => new UserViewModel { Id = x.Id, Name = x.Name, Password = x.Password }).FirstOrDefault();
        return View(model);
    }

    [HttpPost]
    public IActionResult DeletePost(Guid id, [Bind("Id,Name,Password")] UserViewModel model)
    {
        var entity = _userRepository.Get(x => x.Id == id) ?? throw new NullReferenceException();
        entity.IsDeleted = true;
        _userRepository.Update(entity);
        return RedirectToAction(nameof(Index));
    }
}