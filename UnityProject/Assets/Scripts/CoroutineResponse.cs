/** Class to allow getting boolean values from
 * coroutines that get responses from the web. The
 * boolean starts out as null and could remain as
 * null if the connection times out (depends on
 * the implementation of the web-accessing coroutine) */

public class CoroutineResponse
{
    private bool? _success;
    public bool? Success
    {
        get
        {
            return _success;
        }
        private set
        {
            _success = value;
        }
    }

    public CoroutineResponse()
    {
        _success = null;
    }

    public void setSuccess(bool v)
    {
        _success = v;
    }

    /** Resets success to null. Allows for reuse of the
     * same CoroutineResponse object. Only use when
     * disposing of a response that is already processed. */
    public void reset()
    {
        _success = null;
    }
}
