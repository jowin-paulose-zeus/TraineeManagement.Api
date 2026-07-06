using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Services;

public class TraineeService : ITraineeService
{
    private static List<Trainee> trainees = new List<Trainee>
        {
            new Trainee
            {
                Id = 1,
                FirstName = "Jowin",
                LastName = "Paulose",
                Email = "jowin.paulose@zeuslearning.com",
                TechStack = ".NET",
                Status = "Active",
                CreatedDate = new DateTime(2026,7,1,11,30,0),
                UpdatedDate = DateTime.Now
          
            },
            new Trainee
            {
                Id = 2,
                FirstName = "Ishteqali",
                LastName = "Khan",
                Email = "ishteqali.khan@zeuslearning.com",
                TechStack = ".NET",
                Status = "Active",
                CreatedDate = new DateTime(2026,7,1,11,30,0),
                UpdatedDate = DateTime.Now
           
            },
            new Trainee
            {
                Id = 3,
                FirstName = "Siddharth",
                LastName = "Chakravarty",
                Email = "siddharth.chakravarty@zeuslearning.com",
                TechStack = ".NET",
                Status = "Active",
                CreatedDate = new DateTime(2026,7,1,11,30,0),
                UpdatedDate = DateTime.Now            
            }
        };
    public List<TraineeResponseRequest> GetAllTrainees()
        {
            return trainees.Select(t => new TraineeResponseRequest
            {
                Id = t.Id,
                FirstName = t.FirstName,
                LastName = t.LastName,
                Email = t.Email,
                TechStack = t.TechStack,
                Status = t.Status

            }).ToList();

        }
    public TraineeResponseRequest? GetTraineeById(int id)
        {
            var trainee = trainees.FirstOrDefault(t => t.Id == id);

            if (trainee == null)
            {
                return null;
            }
            
            return new TraineeResponseRequest
            {
                Id = trainee.Id,
                FirstName = trainee.FirstName,
                LastName = trainee.LastName,
                Email = trainee.Email,
                TechStack = trainee.TechStack,
                Status = trainee.Status

            };
            
        }
    public TraineeResponseRequest AddTrainee(CreateTraineeRequest request)
        {
            var trainee = new Trainee
            {
                Id = trainees.Count + 1,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                TechStack = request.TechStack,
                Status = request.Status,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };
            
            trainees.Add(trainee);
            
            return new TraineeResponseRequest
            {
                Id = trainee.Id,
                FirstName = trainee.FirstName,
                LastName = trainee.LastName,
                Email = trainee.Email,
                TechStack = trainee.TechStack,
                Status = trainee.Status
            };
        }
    public bool UpdateTraineeData(int id, UpdateTraineeRequest request)
    {
        var trainee = trainees.FirstOrDefault(t => t.Id == id);

            if (trainee == null)
            {
                return false;
            }
        trainee.FirstName = request.FirstName;
        trainee.LastName = request.LastName;
        trainee.Email = request.Email;
        trainee.TechStack = request.TechStack;
        trainee.Status = request.Status;

        return true;
    }
    public bool DeleteTrainee(int id)
    {
        var trainee = trainees.FirstOrDefault(t => t.Id == id);

            if (trainee == null)
            {
                return false;
            }
        
        trainees.Remove(trainee);
        return(true);
    }
}