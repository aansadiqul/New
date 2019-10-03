using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Abp.BackgroundJobs;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Domain.Repositories;
using ABD.ADOrders.Dto;
using ABD.Entities;
using ABD.Hangfire.Dto;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ABD.Hangfire
{
    public class GeoLocationAppService : ABDAppServiceBase, IGeoLocationAppService
    {
        private readonly IRepository<Agency> _agencyRepository;

        private static string BingMapsKey = "AjIwjztEZUGVsVHJ1FXu_Jxg3wsXmErEwQEikWO4-ekKvjr0lCEsWtO-pN7dE3b9";

        public GeoLocationAppService(IRepository<Agency> agencyRepository)
        {
            _agencyRepository = agencyRepository;
        }


        public async Task UpdateGeoLocation()
        {
            var agencies = await _agencyRepository.GetAll()
                .Where(x => x.GeoCodeStatus == null || x.GeoCodeStatus == 1).Take(2).ToListAsync();

            foreach (var item in agencies)
            {
                await GeoCodeAgencyDb(item);
            }

        }

        private async Task GeoCodeAgencyDb(Agency agency)
        {

            GeoCodeing.Response response = new GeoCodeing.Response();
            string requestUrl = CreateRequest(agency, 0);

            response = await MakeRequest(requestUrl, agency);
            if (response != null)
            {
                double latitude;
                double longitude;
                if (response.ResourceSets[0].EstimatedTotal > 0)
                {
                    try
                    {
                        latitude = response.ResourceSets[0].Resources[0].GeocodePoints[0].Coordinates[0];
                        longitude = response.ResourceSets[0].Resources[0].GeocodePoints[0].Coordinates[1];
                        agency.Latitude = latitude;
                        agency.Longitude = longitude;
                        agency.GeoCodeStatus = 2;
                        await UpdateAgency(agency);
                    }
                    catch (Exception e)
                    {
                        await LogAndUpdateException(agency, e);
                    }

                }
                else
                {
                    requestUrl = CreateRequest(agency, 1);
                    response = await MakeRequest(requestUrl, agency);
                    if (response.ResourceSets[0].EstimatedTotal > 0)
                    {
                        try
                        {

                            latitude = response.ResourceSets[0].Resources[0].GeocodePoints[0].Coordinates[0];
                            longitude = response.ResourceSets[0].Resources[0].GeocodePoints[0].Coordinates[1];
                            agency.Longitude = latitude;
                            agency.Longitude = longitude;
                            agency.GeoCodeStatus = 3;
                            await UpdateAgency(agency);
                        }
                        catch (Exception ex)
                        {
                            await LogAndUpdateException(agency, ex);

                        }

                    }
                    else
                    {
                        await LogAndUpdateException(agency, null);
                    }

                }
            }
            else
            {
                await LogAndUpdateException(agency, null);
            }
        }

        private async Task UpdateAgency(Agency agency)
        {
            await _agencyRepository.UpdateAsync(agency);
        }

        private async Task LogAndUpdateException(Agency agency, Exception exception)
        {
            agency.GeoCodeStatus = 1;
            await UpdateAgency(agency);
            var message = exception.Message ?? "No response from server";
            Logger.Error("Error fetching Geo Location for Account Id " + agency.AccountId + " Error Message" + message);
        }

        //Create the request URL
        public static string CreateRequest(Agency agency, int shorturl)
        {
            string UrlRequest = "";
            if (shorturl == 0)
            {
                UrlRequest = "http://dev.virtualearth.net/REST/v1/locations?q=" + HttpUtility.UrlEncode(agency.Address1) + "," + HttpUtility.UrlEncode(agency.City) + "," + "," + HttpUtility.UrlEncode(agency.State) + "," + HttpUtility.UrlEncode(agency.PostalCode) + "&key=" + BingMapsKey;
            }
            else
            {
                UrlRequest = "http://dev.virtualearth.net/REST/v1/locations?q=," + HttpUtility.UrlEncode(agency.City) + "," + "," + HttpUtility.UrlEncode(agency.State) + "," + HttpUtility.UrlEncode(agency.PostalCode) + "&key=" + BingMapsKey;
            }

            return (UrlRequest);
        }

        private async Task<GeoCodeing.Response> MakeRequest(string requestUrl, Agency agency)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                            "Server error (HTTP {0}: {1}).",
                            response.StatusCode,
                            response.StatusDescription));
                    DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(GeoCodeing.Response));
                    object objResponse = jsonSerializer.ReadObject(response.GetResponseStream());
                    GeoCodeing.Response jsonResponse = objResponse as GeoCodeing.Response;
                    return jsonResponse;
                }
            }
            catch (Exception ex)
            {
                await LogAndUpdateException(agency, ex);
                return null;
            }
        }

    }
}
