using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindCabinet.Shared.Utility;


public class Misc {
    public static string GetRandomString( int stringLength ) {
        StringBuilder sb = new StringBuilder();
        int numGuidsToConcat = (((stringLength - 1) / 32) + 1);
        for( int i = 1; i <= numGuidsToConcat; i++ ) {
            sb.Append( Guid.NewGuid().ToString( "N" ) );
        }

        return sb.ToString( 0, stringLength );
    }
}
