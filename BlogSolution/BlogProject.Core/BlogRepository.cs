﻿using BlogProject.Core.Objects;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogProject.Core
{
    public class BlogRepository : IBlogRepository
    {
        // NHibernate object
        private readonly ISession _session;

        public BlogRepository(ISession session)
        {
            _session = session;
        }

        public IList<Post> Posts(int pageNo, int pageSize)
        {
            var posts = _session.Query<Post>()
                                  .Where(p => p.Published)
                                  .OrderByDescending(p => p.PostedOn)
                                  .Skip(pageNo * pageSize)
                                  .Take(pageSize)
                                  .Fetch(p => p.Category)
                                  .ToList();

            var postIds = posts.Select(p => p.Id).ToList();

            return _session.Query<Post>()
                  .Where(p => postIds.Contains(p.Id))
                  .OrderByDescending(p => p.PostedOn)
                  .FetchMany(p => p.Tags)
                  .ToList();
        }

        public int TotalPosts()
        {
            return _session.Query<Post>().Where(p => p.Published).Count();
        }

        public IList<Post> PostsForCategory(string categorySlug, int pageNo, int pageSize)
        {
            var posts = _session.Query<Post>()
                                .Where(p => p.Published && p.Category.UrlSlug.Equals(categorySlug))
                                .OrderByDescending(p => p.PostedOn)
                                .Skip(pageNo * pageSize)
                                .Take(pageSize)
                                .Fetch(p => p.Category)
                                .ToList();

            var postIds = posts.Select(p => p.Id).ToList();

            return _session.Query<Post>()
                          .Where(p => postIds.Contains(p.Id))
                          .OrderByDescending(p => p.PostedOn)
                          .FetchMany(p => p.Tags)
                          .ToList();
        }

        public int TotalPostsForCategory(string categorySlug)
        {
            return _session.Query<Post>()
                        .Where(p => p.Published && p.Category.UrlSlug.Equals(categorySlug))
                        .Count();
        }

        public Category Category(string categorySlug)
        {
            return _session.Query<Category>()
                        .FirstOrDefault(t => t.UrlSlug.Equals(categorySlug));
        }
    }
}