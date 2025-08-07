using System;
using InstagramClone.Domain.Common;
using InstagramClone.Domain.Posts.ValueObjects;

namespace InstagramClone.Domain.Posts.Entities;

public class Post : BaseEntity
{
    public Guid AuthorId { get; private set; }
    public string PostContnet { get; private set; }
    public string ImageUrl { get; private set; }
    private readonly List<TagName> _tags = new();
    public IReadOnlyCollection<TagName> Tags => _tags.AsReadOnly();

    private Post()
    { }

    private Post(Guid authorId, string discription, string imageUrl)
    {
        AuthorId = authorId;
        PostContnet = discription;
        ImageUrl = imageUrl;
    }

    public static Result<Post> Create(Guid authorId, string discription, string imageUrl, List<string> tags)
    {
        var post = new Post(authorId, discription, imageUrl);
        var result = post.AddTags(tags);
        if (result.IsFailure)
            return Result<Post>.Failure(result.Error);

        return Result<Post>.Success(post);
    }

    // ---------- TAGS  -----------------

    /// <summary>
    /// Dodawanie wielu tagów
    /// </summary>

    public Result AddTags(IEnumerable<string> rowTags)
    {
        if (rowTags == null)
            return Result.Failure("Tag list cannot be null.");

        var toAdd = new List<TagName>();
        foreach (var raw in rowTags)
        {
            var tagResult = TagName.Create(raw);
            if (tagResult.IsFailure)
                return Result.Failure($"Tag '{raw}': {tagResult.Error}");

            var tagVo = tagResult.Value;
            if (_tags.Contains(tagVo))
                return Result.Failure($"Tag '{tagVo.Value}' already exists.");

            if (toAdd.Contains(tagVo))
                return Result.Failure($"Duplicate tag in input: '{tagVo.Value}'.");

            toAdd.Add(tagVo);
        }

        foreach (var tagVo in toAdd)
            _tags.Add(tagVo);

        return Result.Success();
    }

    /// <summary>
    /// Dodawanie pojedynczego tagu
    /// </summary>
    public Result AddTag(string tag)
    {
        var tagVo = TagName.Create(tag).Value;
        if (_tags.Contains(tagVo))
            return Result.Failure("Tag already exists.");

        _tags.Add(tagVo);
        return Result.Success();
    }

    /// <summary>
    /// Usunięcie tagu
    /// </summary>
    public Result RemoveTag(string tag)
    {
        var tagVo = TagName.Create(tag).Value;
        if (!_tags.Contains(tagVo))
            return Result.Failure("Tag does not exist.");

        _tags.Remove(tagVo);
        return Result.Success();
    }

}
