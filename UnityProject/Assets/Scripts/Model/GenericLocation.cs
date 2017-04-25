using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GenericLocation : CheckInPlace
{
    const string A = "a ";

    const string ACCOUNTING = "accounting";
    const string AIRPORT = "airport";
    const string AMUSEMENT_PARK = "amusement_park";
    const string AQUARIUM = "aquarium";
    const string ART_GALLERY = "art_gallery";
    const string ATM = "atm";
    const string BAKERY = "bakery";
    const string BANK = "bank";
    const string BAR = "bar";
    const string BEAUTY_SALON = "beauty_salon";
    const string BICYCLE_STORE = "bicycle_store";
    const string BOOK_STORE = "book_store";
    const string BOWLING_ALLEY = "bowling_alley";
    const string BUS_STATION = "bus_station";
    const string CAFE = "cafe";
    const string CAMPGROUND = "campground";
    const string CAR_DEALER = "car_dealer";
    const string CAR_RENTAL = "car_rental";
    const string CAR_REPAIR = "car_repair";
    const string CAR_WASH = "car_wash";
    const string CASINO = "casino";
    const string CEMETERY = "cemetery";
    const string CHURCH = "church";
    const string CITY_HALL = "city_hall";
    const string CLOTHING_STORE = "clothing_store";
    const string CONVENIENCE_STORE = "convenience_store";
    const string COURTHOUSE = "courthouse";
    const string DENTIST = "dentist";
    const string DEPARTMENT_STORE = "department_store";
    const string DOCTOR = "doctor";
    const string ELECTRICIAN = "electrician";
    const string ELECTRONICS_STORE = "electronics_store";
    const string EMBASSY = "embassy";
    const string FIRE_STATION = "fire_station";
    const string FLORIST = "florist";
    const string FUNERAL_HOME = "funeral_home";
    const string FURNITURE_STORE = "furniture_store";
    const string GAS_STATION = "gas_station";
    const string GYM = "gym";
    const string HAIR_CARE = "hair_care";
    const string HARDWARE_STORE = "hardware_store";
    const string HINDU_TEMPLE = "hindu_temple";
    const string HOME_GOODS_STORE = "home_goods_store";
    const string HOSPITAL = "hospital";
    const string INSURANCE_AGENCY = "insurance_agency";
    const string JEWELRY_STORE = "jewelry_store";
    const string LAUNDRY = "laundry";
    const string LAWYER = "lawyer";
    const string LIBRARY = "library";
    const string LIQUOR_STORE = "liquor_store";
    const string LOCAL_GOVERNMENT_OFFICE = "local_government_office";
    const string LOCKSMITH = "locksmith";
    const string LODGING = "lodging";
    const string MEAL_DELIVERY = "meal_delivery";
    const string MEAL_TAKEAWAY = "meal_takeaway";
    const string MOSQUE = "mosque";
    const string MOVIE_RENTAL = "movie_rental";
    const string MOVIE_THEATER = "movie_theater";
    const string MOVING_COMPANY = "moving_company";
    const string MUSEUM = "museum";
    const string NIGHT_CLUB = "night_club";
    const string PAINTER = "painter";
    const string PARK = "park";
    const string PARKING = "parking";
    const string PET_STORE = "pet_store";
    const string PHARMACY = "pharmacy";
    const string PHYSIOTHERAPIST = "physiotherapist";
    const string PLUMBER = "plumber";
    const string POLICE = "police";
    const string POST_OFFICE = "post_office";
    const string REAL_ESTATE_AGENCY = "real_estate_agency";
    const string RESTAURANT = "restaurant";
    const string ROOFING_CONTRACTOR = "roofing_contractor";
    const string RV_PARK = "rv_park";
    const string SCHOOL = "school";
    const string SHOE_STORE = "shoe_store";
    const string SHOPPING_MALL = "shopping_mall";
    const string SPA = "spa";
    const string STADIUM = "stadium";
    const string STORAGE = "storage";
    const string STORE = "store";
    const string SUBWAY_STATION = "subway_station";
    const string SYNAGOGUE = "synagogue";
    const string TAXI_STAND = "taxi_stand";
    const string TRAIN_STATION = "train_station";
    const string TRANSIT_STATION = "transit_station";
    const string TRAVEL_AGENCY = "travel_agency";
    const string UNIVERSITY = "university";
    const string VETERINARY_CARE = "veterinary_care";
    const string ZOO = "zoo";

    public enum GooglePlacesType
    {
        ACCOUNTING
        , AIRPORT
        , AMUSEMENT_PARK
        , AQUARIUM
        , ART_GALLERY
        , ATM
        , BAKERY
        , BANK
        , BAR
        , BEAUTY_SALON
        , BICYCLE_STORE
        , BOOK_STORE
        , BOWLING_ALLEY
        , BUS_STATION
        , CAFE
        , CAMPGROUND
        , CAR_DEALER
        , CAR_RENTAL
        , CAR_REPAIR
        , CAR_WASH
        , CASINO
        , CEMETERY
        , CHURCH
        , CITY_HALL
        , CLOTHING_STORE
        , CONVENIENCE_STORE
        , COURTHOUSE
        , DENTIST
        , DEPARTMENT_STORE
        , DOCTOR
        , ELECTRICIAN
        , ELECTRONICS_STORE
        , EMBASSY
        , FIRE_STATION
        , FLORIST
        , FUNERAL_HOME
        , FURNITURE_STORE
        , GAS_STATION
        , GYM
        , HAIR_CARE
        , HARDWARE_STORE        
        , HINDU_TEMPLE
        , HOME_GOODS_STORE
        , HOSPITAL
        , INSURANCE_AGENCY
        , JEWELRY_STORE
        , LAUNDRY
        , LAWYER
        , LIBRARY
        , LIQUOR_STORE
        , LOCAL_GOVERNMENT_OFFICE
        , LOCKSMITH
        , LODGING
        , MEAL_DELIVERY
        , MEAL_TAKEAWAY
        , MOSQUE
        , MOVIE_RENTAL
        , MOVIE_THEATER
        , MOVING_COMPANY
        , MUSEUM
        , NIGHT_CLUB
        , PAINTER
        , PARK
        , PARKING
        , PET_STORE
        , PHARMACY
        , PHYSIOTHERAPIST
        , PLUMBER
        , POLICE
        , POST_OFFICE
        , REAL_ESTATE_AGENCY
        , RESTAURANT
        , ROOFING_CONTRACTOR
        , RV_PARK
        , SCHOOL
        , SHOE_STORE
        , SHOPPING_MALL
        , SPA
        , STADIUM
        , STORAGE
        , STORE
        , SUBWAY_STATION
        , SYNAGOGUE
        , TAXI_STAND
        , TRAIN_STATION
        , TRANSIT_STATION
        , TRAVEL_AGENCY
        , UNIVERSITY
        , VETERINARY_CARE
        , ZOO
            
            
            
        
        // KEEP THIS HERE    
        , DEFAULT
    }

    public static GooglePlacesType stringToGooglePlacesType(string placeType)
    {
        switch (placeType)
        {
            case ACCOUNTING: return GooglePlacesType.ACCOUNTING;
            case AIRPORT: return GooglePlacesType.AIRPORT;
            case AMUSEMENT_PARK: return GooglePlacesType.AMUSEMENT_PARK;
            case AQUARIUM: return GooglePlacesType.AQUARIUM;
            case ART_GALLERY: return GooglePlacesType.ART_GALLERY;
            case ATM: return GooglePlacesType.ATM;
            case BAKERY: return GooglePlacesType.BAKERY;
            case BANK: return GooglePlacesType.BANK;
            case BAR: return GooglePlacesType.BAR;
            case BEAUTY_SALON: return GooglePlacesType.BEAUTY_SALON;
            case BICYCLE_STORE: return GooglePlacesType.BICYCLE_STORE;
            case BOOK_STORE: return GooglePlacesType.BOOK_STORE;
            case BOWLING_ALLEY: return GooglePlacesType.BOWLING_ALLEY;
            case BUS_STATION: return GooglePlacesType.BUS_STATION;
            case CAFE: return GooglePlacesType.CAFE;
            case CAMPGROUND: return GooglePlacesType.CAMPGROUND;
            case CAR_DEALER: return GooglePlacesType.CAR_DEALER;
            case CAR_RENTAL: return GooglePlacesType.CAR_RENTAL;
            case CAR_REPAIR: return GooglePlacesType.CAR_REPAIR;
            case CAR_WASH: return GooglePlacesType.CAR_WASH;
            case CASINO: return GooglePlacesType.CASINO;
            case CEMETERY: return GooglePlacesType.CEMETERY;
            case CHURCH: return GooglePlacesType.CHURCH;
            case CITY_HALL: return GooglePlacesType.CITY_HALL;
            case CLOTHING_STORE: return GooglePlacesType.CLOTHING_STORE;
            case CONVENIENCE_STORE: return GooglePlacesType.CONVENIENCE_STORE;
            case COURTHOUSE: return GooglePlacesType.COURTHOUSE;
            case DENTIST: return GooglePlacesType.DENTIST;
            case DEPARTMENT_STORE: return GooglePlacesType.DEPARTMENT_STORE;
            case DOCTOR: return GooglePlacesType.DOCTOR;
            case ELECTRICIAN: return GooglePlacesType.ELECTRICIAN;
            case ELECTRONICS_STORE: return GooglePlacesType.ELECTRONICS_STORE;
            case EMBASSY: return GooglePlacesType.EMBASSY;
            case FIRE_STATION: return GooglePlacesType.FIRE_STATION;
            case FLORIST: return GooglePlacesType.FLORIST;
            case FUNERAL_HOME: return GooglePlacesType.FUNERAL_HOME;
            case FURNITURE_STORE: return GooglePlacesType.FURNITURE_STORE;
            case GAS_STATION: return GooglePlacesType.GAS_STATION;
            case GYM: return GooglePlacesType.GYM;
            case HAIR_CARE: return GooglePlacesType.HAIR_CARE;
            case HARDWARE_STORE: return GooglePlacesType.HARDWARE_STORE;
            case HINDU_TEMPLE: return GooglePlacesType.HINDU_TEMPLE;
            case HOME_GOODS_STORE: return GooglePlacesType.HOME_GOODS_STORE;
            case HOSPITAL: return GooglePlacesType.HOSPITAL;
            case INSURANCE_AGENCY: return GooglePlacesType.INSURANCE_AGENCY;
            case JEWELRY_STORE: return GooglePlacesType.JEWELRY_STORE;
            case LAUNDRY: return GooglePlacesType.LAUNDRY;
            case LAWYER: return GooglePlacesType.LAWYER;
            case LIBRARY: return GooglePlacesType.LIBRARY;
            case LIQUOR_STORE: return GooglePlacesType.LIQUOR_STORE;
            case LOCAL_GOVERNMENT_OFFICE: return GooglePlacesType.LOCAL_GOVERNMENT_OFFICE;
            case LOCKSMITH: return GooglePlacesType.LOCKSMITH;
            case LODGING: return GooglePlacesType.LODGING;
            case MEAL_DELIVERY: return GooglePlacesType.MEAL_DELIVERY;
            case MEAL_TAKEAWAY: return GooglePlacesType.MEAL_TAKEAWAY;
            case MOSQUE: return GooglePlacesType.MOSQUE;
            case MOVIE_RENTAL: return GooglePlacesType.MOVIE_RENTAL;
            case MOVIE_THEATER: return GooglePlacesType.MOVIE_THEATER;
            case MOVING_COMPANY: return GooglePlacesType.MOVING_COMPANY;
            case MUSEUM: return GooglePlacesType.MUSEUM;
            case NIGHT_CLUB: return GooglePlacesType.NIGHT_CLUB;
            case PAINTER: return GooglePlacesType.PAINTER;
            case PARK: return GooglePlacesType.PARK;
            case PARKING: return GooglePlacesType.PARKING;
            case PET_STORE: return GooglePlacesType.PET_STORE;
            case PHARMACY: return GooglePlacesType.PHARMACY;
            case PHYSIOTHERAPIST: return GooglePlacesType.PHYSIOTHERAPIST;
            case PLUMBER: return GooglePlacesType.PLUMBER;
            case POLICE: return GooglePlacesType.POLICE;
            case POST_OFFICE: return GooglePlacesType.POST_OFFICE;
            case REAL_ESTATE_AGENCY: return GooglePlacesType.REAL_ESTATE_AGENCY;
            case RESTAURANT: return GooglePlacesType.RESTAURANT;
            case ROOFING_CONTRACTOR: return GooglePlacesType.ROOFING_CONTRACTOR;
            case RV_PARK: return GooglePlacesType.RV_PARK;
            case SCHOOL: return GooglePlacesType.SCHOOL;
            case SHOE_STORE: return GooglePlacesType.SHOE_STORE;
            case SHOPPING_MALL: return GooglePlacesType.SHOPPING_MALL;
            case SPA: return GooglePlacesType.SPA;
            case STADIUM: return GooglePlacesType.STADIUM;
            case STORAGE: return GooglePlacesType.STORAGE;
            case STORE: return GooglePlacesType.STORE;
            case SUBWAY_STATION: return GooglePlacesType.SUBWAY_STATION;
            case SYNAGOGUE: return GooglePlacesType.SYNAGOGUE;
            case TAXI_STAND: return GooglePlacesType.TAXI_STAND;
            case TRAIN_STATION: return GooglePlacesType.TRAIN_STATION;
            case TRANSIT_STATION: return GooglePlacesType.TRANSIT_STATION;
            case TRAVEL_AGENCY: return GooglePlacesType.TRAVEL_AGENCY;
            case UNIVERSITY: return GooglePlacesType.UNIVERSITY;
            case VETERINARY_CARE: return GooglePlacesType.VETERINARY_CARE;
            case ZOO: return GooglePlacesType.ZOO;
            default:
                return GooglePlacesType.DEFAULT;
        }
    }

    public static string googlePlacesTypeToString(GooglePlacesType placeType)
    {
        switch (placeType)
        {
            case GooglePlacesType.ACCOUNTING:                return ACCOUNTING;             
            case GooglePlacesType.AIRPORT:                   return AIRPORT;                
            case GooglePlacesType.AMUSEMENT_PARK:            return AMUSEMENT_PARK;         
            case GooglePlacesType.AQUARIUM:                  return AQUARIUM;               
            case GooglePlacesType.ART_GALLERY:               return ART_GALLERY;            
            case GooglePlacesType.ATM:                       return ATM;                    
            case GooglePlacesType.BAKERY:                    return BAKERY;                 
            case GooglePlacesType.BANK:                      return BANK;                   
            case GooglePlacesType.BAR:                       return BAR;                    
            case GooglePlacesType.BEAUTY_SALON:              return BEAUTY_SALON;           
            case GooglePlacesType.BICYCLE_STORE:             return BICYCLE_STORE;          
            case GooglePlacesType.BOOK_STORE:                return BOOK_STORE;             
            case GooglePlacesType.BOWLING_ALLEY:             return BOWLING_ALLEY;          
            case GooglePlacesType.BUS_STATION:               return BUS_STATION;            
            case GooglePlacesType.CAFE:                      return CAFE;                   
            case GooglePlacesType.CAMPGROUND:                return CAMPGROUND;             
            case GooglePlacesType.CAR_DEALER:                return CAR_DEALER;             
            case GooglePlacesType.CAR_RENTAL:                return CAR_RENTAL;             
            case GooglePlacesType.CAR_REPAIR:                return CAR_REPAIR;             
            case GooglePlacesType.CAR_WASH:                  return CAR_WASH;               
            case GooglePlacesType.CASINO:                    return CASINO;                 
            case GooglePlacesType.CEMETERY:                  return CEMETERY;               
            case GooglePlacesType.CHURCH:                    return CHURCH;                 
            case GooglePlacesType.CITY_HALL:                 return CITY_HALL;              
            case GooglePlacesType.CLOTHING_STORE:            return CLOTHING_STORE;         
            case GooglePlacesType.CONVENIENCE_STORE:         return CONVENIENCE_STORE;      
            case GooglePlacesType.COURTHOUSE:                return COURTHOUSE;             
            case GooglePlacesType.DENTIST:                   return DENTIST;                
            case GooglePlacesType.DEPARTMENT_STORE:          return DEPARTMENT_STORE;       
            case GooglePlacesType.DOCTOR:                    return DOCTOR;                 
            case GooglePlacesType.ELECTRICIAN:               return ELECTRICIAN;            
            case GooglePlacesType.ELECTRONICS_STORE:         return ELECTRONICS_STORE;      
            case GooglePlacesType.EMBASSY:                   return EMBASSY;                
            case GooglePlacesType.FIRE_STATION:              return FIRE_STATION;           
            case GooglePlacesType.FLORIST:                   return FLORIST;                
            case GooglePlacesType.FUNERAL_HOME:              return FUNERAL_HOME;           
            case GooglePlacesType.FURNITURE_STORE:           return FURNITURE_STORE;        
            case GooglePlacesType.GAS_STATION:               return GAS_STATION;            
            case GooglePlacesType.GYM:                       return GYM;                    
            case GooglePlacesType.HAIR_CARE:                 return HAIR_CARE;              
            case GooglePlacesType.HARDWARE_STORE:            return HARDWARE_STORE;         
            case GooglePlacesType.HINDU_TEMPLE:              return HINDU_TEMPLE;           
            case GooglePlacesType.HOME_GOODS_STORE:          return HOME_GOODS_STORE;       
            case GooglePlacesType.HOSPITAL:                  return HOSPITAL;               
            case GooglePlacesType.INSURANCE_AGENCY:          return INSURANCE_AGENCY;       
            case GooglePlacesType.JEWELRY_STORE:             return JEWELRY_STORE;          
            case GooglePlacesType.LAUNDRY:                   return LAUNDRY;                
            case GooglePlacesType.LAWYER:                    return LAWYER;                 
            case GooglePlacesType.LIBRARY:                   return LIBRARY;                
            case GooglePlacesType.LIQUOR_STORE:              return LIQUOR_STORE;           
            case GooglePlacesType.LOCAL_GOVERNMENT_OFFICE:   return LOCAL_GOVERNMENT_OFFICE;
            case GooglePlacesType.LOCKSMITH:                 return LOCKSMITH;              
            case GooglePlacesType.LODGING:                   return LODGING;                
            case GooglePlacesType.MEAL_DELIVERY:             return MEAL_DELIVERY;          
            case GooglePlacesType.MEAL_TAKEAWAY:             return MEAL_TAKEAWAY;          
            case GooglePlacesType.MOSQUE:                    return MOSQUE;                 
            case GooglePlacesType.MOVIE_RENTAL:              return MOVIE_RENTAL;           
            case GooglePlacesType.MOVIE_THEATER:             return MOVIE_THEATER;          
            case GooglePlacesType.MOVING_COMPANY:            return MOVING_COMPANY;         
            case GooglePlacesType.MUSEUM:                    return MUSEUM;                 
            case GooglePlacesType.NIGHT_CLUB:                return NIGHT_CLUB;             
            case GooglePlacesType.PAINTER:                   return PAINTER;                
            case GooglePlacesType.PARK:                      return PARK;                   
            case GooglePlacesType.PARKING:                   return PARKING;                
            case GooglePlacesType.PET_STORE:                 return PET_STORE;              
            case GooglePlacesType.PHARMACY:                  return PHARMACY;               
            case GooglePlacesType.PHYSIOTHERAPIST:           return PHYSIOTHERAPIST;        
            case GooglePlacesType.PLUMBER:                   return PLUMBER;                
            case GooglePlacesType.POLICE:                    return POLICE;                 
            case GooglePlacesType.POST_OFFICE:               return POST_OFFICE;            
            case GooglePlacesType.REAL_ESTATE_AGENCY:        return REAL_ESTATE_AGENCY;     
            case GooglePlacesType.RESTAURANT:                return RESTAURANT;             
            case GooglePlacesType.ROOFING_CONTRACTOR:        return ROOFING_CONTRACTOR;     
            case GooglePlacesType.RV_PARK:                   return RV_PARK;                
            case GooglePlacesType.SCHOOL:                    return SCHOOL;                 
            case GooglePlacesType.SHOE_STORE:                return SHOE_STORE;             
            case GooglePlacesType.SHOPPING_MALL:             return SHOPPING_MALL;          
            case GooglePlacesType.SPA:                       return SPA;                    
            case GooglePlacesType.STADIUM:                   return STADIUM;                
            case GooglePlacesType.STORAGE:                   return STORAGE;                
            case GooglePlacesType.STORE:                     return STORE;                  
            case GooglePlacesType.SUBWAY_STATION:            return SUBWAY_STATION;         
            case GooglePlacesType.SYNAGOGUE:                 return SYNAGOGUE;              
            case GooglePlacesType.TAXI_STAND:                return TAXI_STAND;             
            case GooglePlacesType.TRAIN_STATION:             return TRAIN_STATION;          
            case GooglePlacesType.TRANSIT_STATION:           return TRANSIT_STATION;        
            case GooglePlacesType.TRAVEL_AGENCY:             return TRAVEL_AGENCY;          
            case GooglePlacesType.UNIVERSITY:                return UNIVERSITY;             
            case GooglePlacesType.VETERINARY_CARE:           return VETERINARY_CARE;        
            case GooglePlacesType.ZOO:                       return ZOO;                    
            default:
                return "";
        }
    }

    [SerializeField]
    private string type;

    public GooglePlacesType LocationType
    {
        get
        {
            return stringToGooglePlacesType(type);
        }
        set
        {
            type = googlePlacesTypeToString(value);
            descriptor = A + type.Replace('_', ' ');
        }
    }

    [SerializeField]
    private string descriptor;
    [SerializeField]
    private List<BasicMarker> placesVisited;
    [SerializeField]
    private int requiredVisitCount;

    public GenericLocation(LocationTypeCountTuple t)
    {
        LocationType = t.Type;
        requiredVisitCount = t.RequiredVisitCount;
        placesVisited = new List<BasicMarker>();
    }

    public bool needToBeVisited()
    {
        return placesVisited.Count < requiredVisitCount;
    }

    public void markVisited(BasicMarker marker)
    {
        placesVisited.Add(marker);
    }

    // Adds the visits in gl that do not exist in this GenericLocation
    public void updateVisits(GenericLocation gl)
    {
        foreach (BasicMarker newMarker in gl.placesVisited)
        {
            foreach (BasicMarker oldMarker in placesVisited)
            {
                if (newMarker.Equals(oldMarker))
                    goto nextMarker;
            }
            placesVisited.Add(newMarker);
            nextMarker:;
        }
    }

    public string getDescriptor()
    {
        return descriptor;
    }
}

[System.Serializable]
public class LocationTypeCountTuple
{
    [SerializeField]
    GenericLocation.GooglePlacesType _type;
    public GenericLocation.GooglePlacesType Type
    {
        get { return _type; }
    }

    [SerializeField]
    private int _requiredVisitCount;
    public int RequiredVisitCount
    {
        get { return _requiredVisitCount; }
    }

    public LocationTypeCountTuple(GenericLocation.GooglePlacesType type, int count)
    {
        _type = type;
        _requiredVisitCount = count;
    }
}
