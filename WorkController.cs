using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Model;

namespace Service.Controllers
{
    [Route("api/[controller]")]
    public class WorkController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public WorkController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        [HttpGet]
        public ActionResult Get()
        {
            return Json(DAL.WorkInfo.Instance.GetCount());
        }
        [HttpGet("new")]
        public ActionResult GetNew()
        {
            var result= DAL.WorkInfo.Instance.GetNew();
            if (result.Count() != 0)
                return Json(Result.Ok(result));
            else
                return Json(Result.Err("��¼��Ϊ0"));
        }
        [HttpGet("{id}")]
        public ActionResult Get(int id)  
        {
            var result = DAL.WorkInfo.Instance.GetModel(id);
            if (result != null)
                return Json(Result.Ok(result));
            else
                return Json(Result.Err("WorkId������"));
        }
       
        [HttpPost]
        public ActionResult Post([FromBody] Model.WorkInfo workInfo)  //�����(����»)������src����վ·��ɾ������ֹ��������ַ�仯ʱ���ͻ����޷�����
        { 
            workInfo.recommend = "��";
            workInfo.workVerify = "�����";
            workInfo.uploadTime = DateTime.Now;
            try
            {
                int n = DAL.WorkInfo.Instance.Add(workInfo);
                return Json(Result.Ok("������Ʒ�ɹ�", n));
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("foreign key"))
                    if (ex.Message.ToLower().Contains("username"))
                        return Json(Result.Err("�Ϸ��û�������Ӽ�¼"));
                    else
                        return Json(Result.Err("��Ʒ�����������"));
                else if (ex.Message.ToLower().Contains("null"))
                    return Json(Result.Err("����ơ�����ʱ�䡢�ͼƬ������������û�������Ϊ��"));
                else
                    return Json(Result.Err(ex.Message));
            }
        }
        [HttpPut]
        public ActionResult Put([FromBody] Model.WorkInfo workInfo) 
        {
            workInfo.recommend = "��";
            workInfo.workVerify = "�����";
            workInfo.uploadTime = DateTime.Now;
            try
            {
                var n = DAL.WorkInfo.Instance.Update(workInfo);
                if (n != 0)
                    return Json(Result.Ok("�޸Ļ�ɹ�",workInfo.workId));
                else
                    return Json(Result.Err("workID������"));
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("null"))
                    return Json(Result.Err("����ơ�����ʱ�䡢�ͼƬ�������������Ϊ��"));
                else
                    return Json(Result.Err(ex.Message));
            }
        }
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)  //ɾ���
        {
            try
            {
                var n = DAL.WorkInfo.Instance.Delete(id);
                if (n != 0)
                    return Json(Result.Ok("ɾ���ɹ�"));
                else
                    return Json(Result.Err("activityID������"));

            }
            catch (Exception ex)
            {
                return Json(Result.Err(ex.Message));
            }
        }
        [HttpPost("count")]
        public ActionResult getPage([FromBody]int[] activityIds)
        {
            return Json(DAL.WorkInfo.Instance.GetCount(activityIds));
        }
        [HttpPost("page")]
        public ActionResult getPage([FromBody] Model.WorkPage page)
        {
            var result = DAL.WorkInfo.Instance.GetPage(page);
            if (result.Count() == 0)
            {
                return Json(Result.Err("���ؼ�¼��Ϊ0"));
            }
            else
                return Json(Result.Ok(result));
        }
        [HttpGet("findCount")]
        public ActionResult getFindCount(string findName)
        {
            if (findName == null) findName = "";
            return Json(DAL.WorkInfo.Instance.GetFindCount(findName));
        }
        [HttpGet("myCount")]
        public ActionResult getMyCount(string username)
        {
            if (username == null) username = "";
            return Json(DAL.WorkInfo.Instance.GetMyCount(username));
        }
        [HttpPost("findPage")]
        public ActionResult getFindPage([FromBody] Model.WorkFindPage page)
        {
            if (page.workName == null) page.workName = "";
            var result = DAL.WorkInfo.Instance.GetFindPage(page);
            if (result.Count() == 0)
                return Json(Result.Err("���ؼ�¼��Ϊ0"));
            else
                return Json(Result.Ok(result));
        }
        [HttpPost("myPage")]
        public ActionResult getMyPage([FromBody]Model.WorkMyPage page)
        {
            if (page.userName == null) page.userName = "";
            var result = DAL.WorkInfo.Instance.GetMyPage(page);
            if (result.Count() == 0)
                return Json(Result.Err("���ؼ�¼��Ϊ0"));
            else
                return Json(Result.Ok(result));
        }
        [HttpPut("Verify")]
        public ActionResult PutVerify([FromBody] Model.WorkInfo workInfo)
        {
            try
            {
                var n = DAL.WorkInfo.Instance.UpdateVerify(workInfo);
                if (n != 0)
                    return Json(Result.Ok("�����Ʒ�ɹ�", workInfo.workId));
                else
                    return Json(Result.Err("workId������"));
            }catch(Exception ex)
            {
                if (ex.Message.ToLower().Contains("null"))
                    return Json(Result.Err("��Ʒ����������Ϊ��"));
                else
                    return Json(Result.Err(ex.Message));
            }
        }
        [HttpPut("Recommend")]
        public ActionResult PutRecommend([FromBody] Model.WorkInfo workInfo)
        {
            workInfo.recommendTime = DateTime.Now;
            try
            {
                var re = "";
                if (workInfo.recommend == "��") re = "ȡ��";
                var n = DAL.WorkInfo.Instance.UpdateRecommend(workInfo);
                if (n != 0)
                    return Json(Result.Ok($"{re}�Ƽ���Ʒ�ɹ�", workInfo.workId));
                else
                    return Json(Result.Err("workId������"));
            }catch(Exception ex)
            {
                if (ex.Message.ToLower().Contains("null"))
                    return Json(Result.Err("�Ƽ���Ʒ����Ϊ��"));
                else
                    return Json(Result.Err(ex.Message));
            }
        }
        [HttpPut("{id}")]
        public ActionResult upImg(string id,List<IFormFile> files)
        {
            var path = System.IO.Path.Combine(_hostingEnvironment.WebRootPath, "img", "Work");
            var fileName = $"{path}/{id}";
            try
            {
                var ext = DAL.Upload.Instance.UpImg(files[0], fileName);
                if (ext == null)
                    return Json(Result.Err("���ϴ�ͼƬ�ļ�"));
                else
                {
                    var file = $"img/Work/{id}{ext}";
                    Model.WorkInfo workInfo = new Model.WorkInfo();
                    if (id.StartsWith("i"))
                    {
                        workInfo.workId = int.Parse(id.Substring(1));
                        workInfo.workIntroduction = file;
                    }
                    else
                    {
                        workInfo.workId = int.Parse(id);
                        workInfo.workPicture = file;
                    }
                    var n = DAL.WorkInfo.Instance.UpdateImg(workInfo);
                    if (n > 0)
                        return Json(Result.Ok("�ϴ��ɹ�", file));
                    else
                        return Json(Result.Err("��������ȷ����Ʒid"));
                }
            }
            catch(Exception ex)
            {
                return Json(Result.Err(ex.Message));
            }
        }
    }
}