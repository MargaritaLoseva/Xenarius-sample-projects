using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.OData;
using System.Web.Http.OData.Routing;
using TicketDesk.Domain;
using TicketDesk.Domain.Model;
using System.Web.Http.Cors;

namespace TicketDesk.Web.Client.Controllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using TicketDesk.Domain.Model;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<Ticket>("ODataTickets");
    builder.EntitySet<Project>("Projects"); 
    builder.EntitySet<TicketEvent>("TicketEvents"); 
    builder.EntitySet<TicketSubscriber>("TicketSubscribers"); 
    builder.EntitySet<TicketTag>("TicketTags"); 
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */

    [Authorize]
    [EnableCors("*", "*", "*", SupportsCredentials = true)]
    public class ODataTicketsController : ODataController
    {
        private TdDomainContext db = new TdDomainContext();

        // GET: odata/ODataTickets
        [EnableQuery]
        public IQueryable<Ticket> GetODataTickets()
        {
            return db.Tickets;
        }

        // GET: odata/ODataTickets(5)
        [EnableQuery]
        public SingleResult<Ticket> GetTicket([FromODataUri] int key)
        {
            return SingleResult.Create(db.Tickets.Where(ticket => ticket.TicketId == key));
        }

        // PUT: odata/ODataTickets(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<Ticket> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Ticket ticket = await db.Tickets.FindAsync(key);
            if (ticket == null)
            {
                return NotFound();
            }

            patch.Put(ticket);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TicketExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(ticket);
        }

        // POST: odata/ODataTickets
        public async Task<IHttpActionResult> Post(Ticket ticket)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Tickets.Add(ticket);
            await db.SaveChangesAsync();

            return Created(ticket);
        }

        // PATCH: odata/ODataTickets(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Ticket> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Ticket ticket = await db.Tickets.FindAsync(key);
            if (ticket == null)
            {
                return NotFound();
            }

            patch.Patch(ticket);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TicketExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(ticket);
        }

        // DELETE: odata/ODataTickets(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            Ticket ticket = await db.Tickets.FindAsync(key);
            if (ticket == null)
            {
                return NotFound();
            }

            db.Tickets.Remove(ticket);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/ODataTickets(5)/Project
        [EnableQuery]
        public SingleResult<Project> GetProject([FromODataUri] int key)
        {
            return SingleResult.Create(db.Tickets.Where(m => m.TicketId == key).Select(m => m.Project));
        }

        // GET: odata/ODataTickets(5)/TicketEvents
        [EnableQuery]
        public IQueryable<TicketEvent> GetTicketEvents([FromODataUri] int key)
        {
            return db.Tickets.Where(m => m.TicketId == key).SelectMany(m => m.TicketEvents);
        }

        // GET: odata/ODataTickets(5)/TicketSubscribers
        [EnableQuery]
        public IQueryable<TicketSubscriber> GetTicketSubscribers([FromODataUri] int key)
        {
            return db.Tickets.Where(m => m.TicketId == key).SelectMany(m => m.TicketSubscribers);
        }

        // GET: odata/ODataTickets(5)/TicketTags
        [EnableQuery]
        public IQueryable<TicketTag> GetTicketTags([FromODataUri] int key)
        {
            return db.Tickets.Where(m => m.TicketId == key).SelectMany(m => m.TicketTags);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TicketExists(int key)
        {
            return db.Tickets.Count(e => e.TicketId == key) > 0;
        }
    }
}
