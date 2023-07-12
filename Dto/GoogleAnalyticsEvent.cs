using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gamp.Dto;

internal record GoogleAnalyticsEvent(string Name, object Params);
