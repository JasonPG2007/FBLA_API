using Microsoft.EntityFrameworkCore;
using ObjectBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class MatchDAO
    {
        #region Variables
        private readonly FBLADbContext db;
        #endregion

        #region Constructor
        public MatchDAO(FBLADbContext db)
        {
            this.db = db;
        }
        #endregion

        #region Create Match
        public async Task<bool> CreateMatch(Match match)
        {
            match.MatchId = new Random().Next();
            var isAdded = db.Match.Add(match);
            if (isAdded != null)
            {
                await db.SaveChangesAsync();
                return true;
            }
            return false;
        }
        #endregion

        #region All matches
        public IQueryable<Match> AllMatches()
        {
            var listMatches = db.Match.AsNoTracking();
            return listMatches;
        }
        #endregion

        #region Get Match By Id
        public Match GetMatchById(int matchId)
        {
            var match = db.Match.FirstOrDefault(m => m.MatchId == matchId);
            return match;
        }
        #endregion

        #region Update match
        public async Task<bool> UpdateMatch(Match match)
        {
            return false;
        }
        #endregion

        #region Delete match
        public async Task<bool> DeleteMatch(int matchId)
        {
            return false;
        }
        #endregion
    }
}
