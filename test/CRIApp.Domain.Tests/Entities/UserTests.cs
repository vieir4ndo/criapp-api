using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;
using CRIApp.Domain.Exceptions;
using Xunit;
using System.Reflection;

namespace CRIApp.Domain.Tests.Entities;

public class UserTests
{
    private static User CreateValidUser(string username = "test@example.com")
        => new User("Test User", new DateTime(1990, 1, 1), Sex.Male, username, "password123");

    private static readonly Guid AdminId = Guid.NewGuid();

    private static Answer BuildAnswerWithQuestion(int value, QuestionScoringType scoringType)
    {
        var question = new Question("Sample question?", AdminId, QuestionType.Agreement, scoringType);
        var answer = new Answer(value, question.Id, Guid.NewGuid(), Guid.NewGuid());
        typeof(Answer).GetProperty(nameof(Answer.Question))!.SetValue(answer, question);
        return answer;
    }

    [Fact]
    public void Create_ValidUserWithEmail_ShouldSucceed()
    {
        var user = CreateValidUser("user@domain.com");

        Assert.NotNull(user);
        Assert.Equal("Test User", user.Name);
        Assert.True(user.Active);
        Assert.Equal(Sex.Male, user.Sex);
    }

    [Fact]
    public void Create_ValidUserWithPhone_ShouldSucceed()
    {
        var user = CreateValidUser("+34612345678");

        Assert.NotNull(user);
        Assert.True(user.Active);
    }

    [Fact]
    public void Create_WithEmailUsername_ShouldReturnEmailUsernameType()
    {
        var user = CreateValidUser("user@domain.com");

        Assert.Equal(UsernameType.Email, user.UsernameType());
    }

    [Fact]
    public void Create_WithPhoneUsername_ShouldReturnPhoneNumberUsernameType()
    {
        var user = CreateValidUser("+34612345678");

        Assert.Equal(UsernameType.PhoneNumber, user.UsernameType());
    }

    [Fact]
    public void Create_WithEmptyName_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new User("", new DateTime(1990, 1, 1), Sex.Male, "user@domain.com", "password123"));
    }

    [Fact]
    public void Create_WithNullName_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new User(null!, new DateTime(1990, 1, 1), Sex.Male, "user@domain.com", "password123"));
    }

    [Fact]
    public void Create_WithMinValueBirthDate_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new User("Test", DateTime.MinValue, Sex.Male, "user@domain.com", "password123"));
    }

    [Fact]
    public void Create_WithInvalidUsername_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new User("Test", new DateTime(1990, 1, 1), Sex.Male, "not-valid-username", "password123"));
    }

    [Fact]
    public void VerifyPassword_CorrectPassword_ShouldReturnTrue()
    {
        var user = CreateValidUser();

        Assert.True(user.VerifyPassword("password123"));
    }

    [Fact]
    public void VerifyPassword_WrongPassword_ShouldReturnFalse()
    {
        var user = CreateValidUser();

        Assert.False(user.VerifyPassword("wrongpassword"));
    }

    [Fact]
    public void Deactivate_ShouldSetActiveToFalse()
    {
        var user = CreateValidUser();

        user.Deactivate();

        Assert.False(user.Active);
    }

    [Fact]
    public void Activate_AfterDeactivate_ShouldSetActiveToTrue()
    {
        var user = CreateValidUser();
        user.Deactivate();

        user.Activate();

        Assert.True(user.Active);
    }

    [Fact]
    public void Update_WithValidData_ShouldUpdateProperties()
    {
        var user = CreateValidUser();
        var newBirthDate = new DateTime(1995, 5, 10);

        user.Update("New Name", newBirthDate, Sex.Female, EducationalLevel.University, FamilyType.HeterosexualCouple, 2);

        Assert.Equal("New Name", user.Name);
        Assert.Equal(newBirthDate, user.BirthDate);
        Assert.Equal(Sex.Female, user.Sex);
        Assert.Equal(EducationalLevel.University, user.EducationalLevel);
        Assert.Equal(FamilyType.HeterosexualCouple, user.FamilyType);
        Assert.Equal(2, user.NumberOfChildren);
    }

    [Fact]
    public void Update_WithNullValues_ShouldKeepExistingValues()
    {
        var user = CreateValidUser();
        var originalName = user.Name;
        var originalBirthDate = user.BirthDate;

        user.Update(null, null, null, null, null, null);

        Assert.Equal(originalName, user.Name);
        Assert.Equal(originalBirthDate, user.BirthDate);
    }

    [Fact]
    public void UpdatePassword_WithNewPassword_ShouldHashAndUpdate()
    {
        var user = CreateValidUser();

        user.UpdatePassword("newpassword456");

        Assert.True(user.VerifyPassword("newpassword456"));
        Assert.False(user.VerifyPassword("password123"));
    }

    [Fact]
    public void VerifyResetPasswordCode_NoCodesExist_ShouldReturnFalse()
    {
        var user = CreateValidUser();

        Assert.False(user.VerifyResetPasswordCode("anycode"));
    }

    [Fact]
    public void VerifyResetPasswordCode_ValidCodeNotExpired_ShouldReturnTrue()
    {
        var user = CreateValidUser();
        var code = new UserResetPasswordCode(user.Id, "VALID", DateTime.UtcNow.AddSeconds(180));
        user.UserResetPasswordCodes.Add(code);

        Assert.True(user.VerifyResetPasswordCode("VALID"));
    }

    [Fact]
    public void VerifyResetPasswordCode_ExpiredCode_ShouldReturnFalse()
    {
        var user = CreateValidUser();
        var code = new UserResetPasswordCode(user.Id, "EXPIRED", DateTime.UtcNow.AddSeconds(-1));
        user.UserResetPasswordCodes.Add(code);

        Assert.False(user.VerifyResetPasswordCode("EXPIRED"));
    }

    [Fact]
    public void VerifyResetPasswordCode_WrongCode_ShouldReturnFalse()
    {
        var user = CreateValidUser();
        var code = new UserResetPasswordCode(user.Id, "CORRECT", DateTime.UtcNow.AddSeconds(180));
        user.UserResetPasswordCodes.Add(code);

        Assert.False(user.VerifyResetPasswordCode("WRONG"));
    }

    [Fact]
    public void UpdateScore_NoAnswers_ShouldSetScoreToZero()
    {
        var user = CreateValidUser();

        user.UpdateScore();

        Assert.Equal(0, user.Score);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void UpdateScore_DefaultScoringWithCountableValue_ShouldIncrementScore(int value)
    {
        var user = CreateValidUser();
        user.Answers.Add(BuildAnswerWithQuestion(value, QuestionScoringType.Default));

        user.UpdateScore();

        Assert.Equal(100, user.Score);
    }

    [Fact]
    public void UpdateScore_DefaultScoringWithValue5_ShouldNotCount()
    {
        var user = CreateValidUser();
        user.Answers.Add(BuildAnswerWithQuestion(5, QuestionScoringType.Default));

        user.UpdateScore();

        Assert.Equal(0, user.Score);
    }

    [Fact]
    public void UpdateScore_ReversedScoringWithValue5_ShouldIncrementScore()
    {
        var user = CreateValidUser();
        user.Answers.Add(BuildAnswerWithQuestion(5, QuestionScoringType.Reversed));

        user.UpdateScore();

        Assert.Equal(100, user.Score);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void UpdateScore_ReversedScoringWithLowValue_ShouldNotCount(int value)
    {
        var user = CreateValidUser();
        user.Answers.Add(BuildAnswerWithQuestion(value, QuestionScoringType.Reversed));

        user.UpdateScore();

        Assert.Equal(0, user.Score);
    }

    [Fact]
    public void UpdateScore_MixedScoringTypes_ShouldCountCorrectly()
    {
        var user = CreateValidUser();
        user.Answers.Add(BuildAnswerWithQuestion(3, QuestionScoringType.Default));   // correct
        user.Answers.Add(BuildAnswerWithQuestion(5, QuestionScoringType.Default));   // wrong
        user.Answers.Add(BuildAnswerWithQuestion(5, QuestionScoringType.Reversed));  // correct
        user.Answers.Add(BuildAnswerWithQuestion(2, QuestionScoringType.Reversed));  // wrong

        user.UpdateScore();

        Assert.Equal(50, user.Score);
    }

    [Fact]
    public void UpdateScore_ShouldUpdateScoreLastUpdatedAt()
    {
        var user = CreateValidUser();
        var before = DateTime.UtcNow;

        user.UpdateScore();

        Assert.True(user.ScoreLastUpdatedAt >= before);
    }

    [Fact]
    public void AddAwardIfNeeded_NewAwardType_ShouldAddAward()
    {
        var user = CreateValidUser();

        user.AddAwardIfNeeded(AwardType.FirstSteps);

        Assert.Single(user.Awards);
        Assert.Equal(AwardType.FirstSteps, user.Awards[0].Type);
    }

    [Fact]
    public void AddAwardIfNeeded_SameAwardTypeTwice_ShouldNotDuplicate()
    {
        var user = CreateValidUser();

        user.AddAwardIfNeeded(AwardType.FirstSteps);
        user.AddAwardIfNeeded(AwardType.FirstSteps);

        Assert.Single(user.Awards);
    }

    [Fact]
    public void AddAwardIfNeeded_DifferentAwardTypes_ShouldAddBoth()
    {
        var user = CreateValidUser();

        user.AddAwardIfNeeded(AwardType.FirstSteps);
        user.AddAwardIfNeeded(AwardType.EngagedFamily);

        Assert.Equal(2, user.Awards.Count);
    }

    [Fact]
    public void AddAwardIfNeeded_AllFiveTypes_ShouldAddAllFive()
    {
        var user = CreateValidUser();

        user.AddAwardIfNeeded(AwardType.FirstSteps);
        user.AddAwardIfNeeded(AwardType.EngagedFamily);
        user.AddAwardIfNeeded(AwardType.Overcoming);
        user.AddAwardIfNeeded(AwardType.PositiveEnvironment);
        user.AddAwardIfNeeded(AwardType.SmallProgress);

        Assert.Equal(5, user.Awards.Count);
    }
}
