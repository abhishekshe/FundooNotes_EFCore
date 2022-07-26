﻿using DataBaseLayer.LabelModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interface
{
    public interface ILabelBL
    {
        Task AddLabel(int UserId, int NoteId, string LabelName);

        Task<List<LabelModel>> GetAllLabels(int UserId);

        Task<List<LabelModel>> GetLabelByNoteId(int UserId, int NoteId);

        Task<bool> UpdateLable(int NoteId, string Labelname);
    }
}