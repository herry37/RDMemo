// 系統命名空間
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Linq.Expressions;
global using System.Text;
global using System.Text.Json;
global using System.Threading;
global using System.Threading.Tasks;
global using System.Security.Claims;
global using System.Security.Cryptography;
global using System.IO;
global using System.Diagnostics;
global using System.Reflection;
global using System.ComponentModel.DataAnnotations;
global using System.Net;
global using System.IdentityModel.Tokens.Jwt;
global using System.Globalization;

// Entity Framework Core
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Diagnostics;
global using Microsoft.EntityFrameworkCore.Infrastructure;
global using Microsoft.EntityFrameworkCore.Storage;
global using Microsoft.EntityFrameworkCore.Query;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using Microsoft.EntityFrameworkCore.Design;

// Microsoft Extensions
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using Microsoft.Extensions.Caching.Memory;
global using Microsoft.Extensions.Diagnostics.HealthChecks;
global using Microsoft.IdentityModel.Tokens;

// ASP.NET Core
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.ResponseCompression;

// 第三方套件
global using MediatR;
global using StackExchange.Redis;
global using Prometheus;
global using Newtonsoft.Json;
global using Newtonsoft.Json.Linq;
global using Serilog;
global using Serilog.Events;

// Domain 層
global using BackendManagement.Domain.Common;
global using BackendManagement.Domain.Common.Interfaces;
global using BackendManagement.Domain.Entities;
global using BackendManagement.Domain.Events;
global using BackendManagement.Domain.Interfaces;
global using BackendManagement.Domain.MultiTenancy;
global using BackendManagement.Domain.Resilience;

// Application 層
global using BackendManagement.Application.Common.Interfaces;
global using BackendManagement.Application.Common.Models;

// Infrastructure 層
global using BackendManagement.Infrastructure.Persistence;
global using BackendManagement.Infrastructure.Persistence.Interceptors;
global using BackendManagement.Infrastructure.Services;
global using BackendManagement.Infrastructure.MultiTenancy;
global using BackendManagement.Infrastructure.Monitoring;
global using BackendManagement.Infrastructure.Authentication;
global using BackendManagement.Infrastructure.Performance;
global using BackendManagement.Infrastructure.Resilience;
global using BackendManagement.Infrastructure.Logging;
global using BackendManagement.Infrastructure.Data;