﻿using BusinessLayer.Interface;
using DataBaseLayer.LabelModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NLoggers.Interface;
using RepositoryLayer.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FundooNotes_EFCore.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class LabelController : ControllerBase
    {
        private readonly FundooContext fundooContext;
        private readonly ILabelBL labelBL;
        private readonly ILoggerManager logger;

        public LabelController(FundooContext fundooContext, ILabelBL labelBL, ILoggerManager logger)
        {
            this.fundooContext = fundooContext;
            this.labelBL = labelBL;
            this.logger = logger;
        }

        [HttpPost("AddLabel/{NoteId}/{Labelname}")]
        public async Task<IActionResult> AddLabel(int NoteId, string Labelname)
        {

            try
            {
                var userId = User.Claims.FirstOrDefault(x => x.Type.ToString().Equals("UserId", StringComparison.InvariantCultureIgnoreCase));
                int UserId = int.Parse(userId.Value);
                var note = this.fundooContext.Notes.FirstOrDefault(x => x.NoteId == NoteId);
                var label = this.fundooContext.Label.FirstOrDefault(x => x.LabelName == Labelname);

                if (note == null || note.IsTrash == true)
                {
                    this.logger.LogInfo($"Enterd invalid Note Id {NoteId}");
                    return this.BadRequest(new { success = false, Message = "Enter valid NoteId" });
                }

                if (label == null)
                {
                    await this.labelBL.AddLabel(UserId, NoteId, Labelname);
                    this.logger.LogInfo($"Label Cread Successfully with noted id = {NoteId}");
                    return this.Ok(new { sucess = true, Message = "Label Created Successfully..." });
                }

                return this.BadRequest(new { sucess = false, Message = "Label with the name already Exists !!" });
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex.Message);
                throw ex;
            }
        }

        [HttpGet("GetAllLabels")]
        public async Task<IActionResult> GetAllLabels()
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(x => x.Type.ToString().Equals("UserId", StringComparison.InvariantCultureIgnoreCase));
                int UserId = int.Parse(userId.Value);
                var result = await this.labelBL.GetAllLabels(UserId);
                if (result.Count > 0)
                {
                    return this.Ok(new { sucess = true, Message = "Fetch all labels", data = result });
                }
                return this.Ok(new { sucess = true, Message = "No Labels Found", data = result });

            }
            catch (Exception ex)
            {
                this.logger.LogError(ex.Message);
                throw ex;
            }
        }

        [HttpGet("GetAllLabels/{NoteId}")]
        public async Task<IActionResult> GetAllLabelsByNote(int NoteId)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(x => x.Type.ToString().Equals("UserId", StringComparison.InvariantCultureIgnoreCase));
                int UserId = int.Parse(userId.Value);
                var result = await this.labelBL.GetLabelByNoteId(UserId, NoteId);
                if (result.Count > 0)
                {
                    return this.Ok(new { sucess = true, Message = "Fetch all labels", data = result });
                }
                return this.BadRequest(new { sucess = false, Message = "No Labels Found" });

            }
            catch (Exception ex)
            {
                this.logger.LogError(ex.Message);
                throw ex;
            }
        }

        [HttpPut("UpdateLabel/{LabelId}/{Labelname}")]
        public async Task<IActionResult> UpdatedLabel(int LabelId, string Labelname)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(x => x.Type.ToString().Equals("UserId", StringComparison.InvariantCultureIgnoreCase));
                int UserId = int.Parse(userId.Value);
                var label = this.fundooContext.Label.FirstOrDefault(x => x.LabelId == LabelId && x.UserId == UserId);
                if (label == null)
                {
                    return this.BadRequest(new { sucess = false, Message = "Enter valid NoteId" });
                }

                bool result = await this.labelBL.UpdateLable(UserId, LabelId, Labelname);
                if (result)
                {
                    return this.Ok(new { sucess = true, Message = "Updated Label Successfully! " });
                }

                return this.BadRequest(new { sucess = false, Message = "Entered Label Name already exsists!!" });
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex.Message);
                throw ex;
            }
        }

        [HttpDelete("DeleteLabel/{LabelId}")]
        public async Task<IActionResult> DeleteLabel(int LabelId)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(x => x.Type.ToString().Equals("UserId", StringComparison.InvariantCultureIgnoreCase));
                int UserId = int.Parse(userId.Value);
                bool result = await this.labelBL.DeleteLabel(UserId, LabelId);
                if (result)
                {
                    return this.Ok(new { sucess = true, Message = "Deleted SuccessFully !! " });
                }

                return this.BadRequest(new { sucess = false, Message = "Enter Valid Label Id !!" });
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex.Message);
                throw ex;
            }
        }
    }
}