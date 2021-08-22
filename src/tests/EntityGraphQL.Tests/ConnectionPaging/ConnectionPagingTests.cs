using System.Linq;
using EntityGraphQL.Schema;
using EntityGraphQL.Schema.FieldExtensions;
using Xunit;

namespace EntityGraphQL.Tests.ConnectionPaging
{
    public class ConnectionPagingTests
    {
        private static int peopleCnt;

        [Fact]
        public void TestGetsAll()
        {
            var schema = SchemaBuilder.FromObject<TestSchema>();
            var data = new TestSchema();
            FillData(data);

            schema.ReplaceField("people", ctx => ctx.People.OrderBy(p => p.Id), "Return list of people with paging metadata")
                .UseConnectionPaging();
            var gql = new QueryRequest
            {
                Query = @"{
                    people {
                        edges {
                            node {
                                name id
                            }
                            cursor
                        }
                        pageInfo {
                            startCursor
                            endCursor
                            hasNextPage
                            hasPreviousPage
                        }
                        totalCount
                    }
                }",
            };

            var result = schema.ExecuteQuery(gql, data, null, null);
            Assert.Null(result.Errors);

            dynamic people = result.Data["people"];
            Assert.Equal(data.People.Count, Enumerable.Count(people.edges));
            Assert.Equal(data.People.Count, people.totalCount);
            Assert.False(people.pageInfo.hasNextPage);
            Assert.False(people.pageInfo.hasPreviousPage);

            // cursors MQ, Mg, Mw, NA, NQ

            // we have tests for (de)serialization of cursor we're checking the correct ones are used
            var expectedFirstCursor = ConnectionHelper.SerializeCursor(1);
            var expectedLastCursor = ConnectionHelper.SerializeCursor(Enumerable.Count(people.edges));
            Assert.Equal(expectedFirstCursor, people.pageInfo.startCursor);
            Assert.Equal(expectedLastCursor, people.pageInfo.endCursor);
            Assert.Equal(expectedFirstCursor, Enumerable.First(people.edges).cursor);
            Assert.Equal(expectedLastCursor, Enumerable.Last(people.edges).cursor);
        }

        [Fact]
        public void TestFirst()
        {
            var schema = SchemaBuilder.FromObject<TestSchema>();
            var data = new TestSchema();
            FillData(data);

            schema.ReplaceField("people", ctx => ctx.People.OrderBy(p => p.Id), "Return list of people with paging metadata")
                .UseConnectionPaging();
            var gql = new QueryRequest
            {
                Query = @"{
                    people(first: 1) {
                        edges {
                            node {
                                name id
                            }
                            cursor
                        }
                        pageInfo {
                            startCursor
                            endCursor
                            hasNextPage
                            hasPreviousPage
                        }
                        totalCount
                    }
                }",
            };

            var result = schema.ExecuteQuery(gql, data, null, null);
            Assert.Null(result.Errors);

            dynamic people = result.Data["people"];
            Assert.Equal(1, Enumerable.Count(people.edges));
            Assert.Equal(data.People.Count, people.totalCount);
            Assert.True(people.pageInfo.hasNextPage);
            Assert.False(people.pageInfo.hasPreviousPage);

            // cursors MQ, Mg, Mw, NA, NQ

            // we have tests for (de)serialization of cursor we're checking the correct ones are used
            var expectedFirstCursor = ConnectionHelper.SerializeCursor(1);
            var expectedLastCursor = expectedFirstCursor;
            Assert.Equal(expectedFirstCursor, people.pageInfo.startCursor);
            Assert.Equal(expectedLastCursor, people.pageInfo.endCursor);
            Assert.Equal(expectedFirstCursor, Enumerable.First(people.edges).cursor);
            Assert.Equal(expectedLastCursor, Enumerable.Last(people.edges).cursor);
        }
        [Fact]
        public void TestFirstAfter()
        {
            var schema = SchemaBuilder.FromObject<TestSchema>();
            var data = new TestSchema();
            FillData(data);

            schema.ReplaceField("people", ctx => ctx.People.OrderBy(p => p.Id), "Return list of people with paging metadata")
                .UseConnectionPaging();
            var gql = new QueryRequest
            {
                Query = @"{
                    people(first: 2 after: ""MQ=="") {
                        edges {
                            node {
                                name id
                            }
                            cursor
                        }
                        pageInfo {
                            startCursor
                            endCursor
                            hasNextPage
                            hasPreviousPage
                        }
                        totalCount
                    }
                }",
            };

            var result = schema.ExecuteQuery(gql, data, null, null);
            Assert.Null(result.Errors);

            dynamic people = result.Data["people"];
            Assert.Equal(2, Enumerable.Count(people.edges));
            Assert.Equal(data.People.Count, people.totalCount);
            Assert.True(people.pageInfo.hasNextPage);
            Assert.True(people.pageInfo.hasPreviousPage);

            // cursors MQ, Mg, Mw, NA, NQ

            // we have tests for (de)serialization of cursor we're checking the correct ones are used
            var expectedFirstCursor = ConnectionHelper.SerializeCursor(2);
            var expectedLastCursor = ConnectionHelper.SerializeCursor(3);
            Assert.Equal(expectedFirstCursor, people.pageInfo.startCursor);
            Assert.Equal(expectedLastCursor, people.pageInfo.endCursor);
            Assert.Equal(expectedFirstCursor, Enumerable.First(people.edges).cursor);
            Assert.Equal(expectedLastCursor, Enumerable.Last(people.edges).cursor);
        }
        [Fact]
        public void TestLast()
        {
            var schema = SchemaBuilder.FromObject<TestSchema>();
            var data = new TestSchema();
            FillData(data);

            schema.ReplaceField("people", ctx => ctx.People.OrderBy(p => p.Id), "Return list of people with paging metadata")
                .UseConnectionPaging();
            var gql = new QueryRequest
            {
                Query = @"{
                    people(last: 2) {
                        edges {
                            node {
                                name id
                            }
                            cursor
                        }
                        pageInfo {
                            startCursor
                            endCursor
                            hasNextPage
                            hasPreviousPage
                        }
                        totalCount
                    }
                }",
            };

            var result = schema.ExecuteQuery(gql, data, null, null);
            Assert.Null(result.Errors);

            dynamic people = result.Data["people"];
            Assert.Equal(2, Enumerable.Count(people.edges));
            Assert.Equal(data.People.Count, people.totalCount);
            Assert.False(people.pageInfo.hasNextPage);
            Assert.True(people.pageInfo.hasPreviousPage);

            // cursors MQ, Mg, Mw, NA, NQ

            // we have tests for (de)serialization of cursor we're checking the correct ones are used
            var expectedFirstCursor = ConnectionHelper.SerializeCursor(4);
            var expectedLastCursor = ConnectionHelper.SerializeCursor(5);
            Assert.Equal(expectedFirstCursor, people.pageInfo.startCursor);
            Assert.Equal(expectedLastCursor, people.pageInfo.endCursor);
            Assert.Equal(expectedFirstCursor, Enumerable.First(people.edges).cursor);
            Assert.Equal(expectedLastCursor, Enumerable.Last(people.edges).cursor);
        }
        [Fact]
        public void TestLastBefore()
        {
            var schema = SchemaBuilder.FromObject<TestSchema>();
            var data = new TestSchema();
            FillData(data);

            schema.ReplaceField("people", ctx => ctx.People.OrderBy(p => p.Id), "Return list of people with paging metadata")
                .UseConnectionPaging();
            var gql = new QueryRequest
            {
                Query = @"{
                    people(last: 3 before: ""NA=="") {
                        edges {
                            node {
                                name id
                            }
                            cursor
                        }
                        pageInfo {
                            startCursor
                            endCursor
                            hasNextPage
                            hasPreviousPage
                        }
                        totalCount
                    }
                }",
            };

            var result = schema.ExecuteQuery(gql, data, null, null);
            Assert.Null(result.Errors);

            dynamic people = result.Data["people"];
            Assert.Equal(3, Enumerable.Count(people.edges));
            Assert.Equal(data.People.Count, people.totalCount);
            Assert.True(people.pageInfo.hasNextPage);
            Assert.False(people.pageInfo.hasPreviousPage);

            // cursors MQ, Mg, Mw, NA, NQ

            // we have tests for (de)serialization of cursor we're checking the correct ones are used
            var expectedFirstCursor = ConnectionHelper.SerializeCursor(1);
            var expectedLastCursor = ConnectionHelper.SerializeCursor(3);
            Assert.Equal(expectedFirstCursor, people.pageInfo.startCursor);
            Assert.Equal(expectedLastCursor, people.pageInfo.endCursor);
            Assert.Equal(expectedFirstCursor, Enumerable.First(people.edges).cursor);
            Assert.Equal(expectedLastCursor, Enumerable.Last(people.edges).cursor);
        }
        private static void FillData(TestSchema data)
        {
            data.People = new()
            {
                MakePerson("Bill", "Murray"),
                MakePerson("John", "Frank"),
                MakePerson("Cheryl", "Crow"),
                MakePerson("Jill", "Castle"),
                MakePerson("Jack", "Snider"),
            };
        }

        private static Person MakePerson(string fname, string lname)
        {
            return new Person
            {
                Id = peopleCnt++,
                Name = fname,
                LastName = lname
            };
        }
    }
}