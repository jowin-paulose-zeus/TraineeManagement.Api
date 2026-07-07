using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.Services
{
public class TraineeService : ITraineeService
{
    private static List<Trainee> trainees =
    [
            new Trainee
            {
                Id = 1,
                FirstName = "Jowin",
                LastName = "Paulose",
                Email = "jowin.paulose@zeuslearning.com",
                TechStack = ".NET",
                Status = TraineeStatus.Active,
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
                Status = TraineeStatus.Active,
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
                Status = TraineeStatus.Active,
                CreatedDate = new DateTime(2026,7,1,11,30,0),
                UpdatedDate = DateTime.Now
            }
        ];
    
    private static TraineeResponseRequest MapToResponse(Trainee trainee)
    {
        return new TraineeResponseRequest
        {
            Id = trainee.Id,
            FirstName = trainee.FirstName,
            LastName = trainee.LastName,
            Email = trainee.Email,
            TechStack = trainee.TechStack,
            Status = trainee.Status.ToString()
        };
    }
    public List<TraineeResponseRequest> GetAllTrainees()
    {
        return trainees.Select(MapToResponse).ToList();
    }
    public TraineeResponseRequest? GetTraineeById(int id)
    {
        var trainee = trainees.FirstOrDefault(t => t.Id == id);

        if (trainee == null)
        {
            return null;
        }

        return MapToResponse(trainee);
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

        return MapToResponse(trainee);
    }

    public bool UpdateTraineeData(int id, UpdateTraineeRequest request)
    {
        var trainee = trainees.FirstOrDefault(t => t.Id == id);

        if (trainee == null)
        {
            return false;
        }
        MapToResponse(trainee);
        trainee.UpdatedDate = DateTime.Now;
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
        return true;
    }
}
}