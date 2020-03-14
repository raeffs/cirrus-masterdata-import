using System;

namespace Cirrus.Import.Masterdata.Cirrus
{
    class UpdateFailedException : Exception
    {
        public UpdateFailedException()
            : base("Not all properties of the view model were updated")
        {
        }
    }
}
