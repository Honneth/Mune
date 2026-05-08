SELECT up.Id, up.Headline, us.UserName
FROM dbo.UserPosts up
JOIN dbo.AspNetUsers us
ON up.UserId = us.Id