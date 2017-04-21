using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.models.archive
{
    [Serializable]
    public class ArchiveLearningQuality : ArchiveModel
    {
        private int mistakeTrain;
        public int MistakeTrain
        {
            get
            {
                return mistakeTrain;
            }

            set
            {
                mistakeTrain = value;
            }
        }

        private int mistakeTest;
        public int MistakeTest
        {
            get
            {
                return mistakeTest;
            }

            set
            {
                mistakeTest = value;
            }
        }

        private double closingError;
        public double ClosingError
        {
            get
            {
                return closingError;
            }
            set
            {
                closingError = value;
            }
        }

        public ArchiveLearningQuality(models.LearningQuality q)
        {
            this.ID = q.ID;
            this.MistakeTest = q.MistakeTest;
            this.MistakeTrain = q.MistakeTrain;
            this.ClosingError = q.ClosingError;
        }
    }
}
