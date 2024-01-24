using FlyingDuchmanAirlines.ControllerLayer.JsonData;

using Microsoft.AspNetCore.Mvc.ModelBinding;

using System.Buffers;
using System.IO.Pipelines;
using System.Text;
using System.Text.Json;

namespace FlyingDuchmanAirlines.ControllerLayer
{
    internal class BookingModelBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            //! 1. check to see if it's null or not

            if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));

            //! 2. Read the HTTP body into a parsable format

            ReadResult result = await bindingContext.HttpContext.Request.BodyReader.ReadAsync();

            ReadOnlySequence<byte> buffer = result.Buffer;
            string body = Encoding.UTF8.GetString(buffer.FirstSpan);

            //! 3. bind the HTTP body data to the class you want

            BookingData data = JsonSerializer.Deserialize<BookingData>(body)!;

            //! 4. return the bound model

            bindingContext.Result = ModelBindingResult.Success(data);
        }
    }
}
