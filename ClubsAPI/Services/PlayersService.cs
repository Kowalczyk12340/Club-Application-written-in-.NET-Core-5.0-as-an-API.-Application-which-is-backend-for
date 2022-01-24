using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClubsAPI.Data;
using ClubsAPI.DTOs;
using ClubsAPI.Entities;
using ClubsAPI.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClubsAPI.Services.Interfaces;
using ClubsAPI.Exceptions;
using System;
using System.Text;

namespace ClubsAPI.Services
{
  public class PlayersService : IPlayersService
  {
    private readonly ApplicationDataContext _context;
    private readonly IMapper _mapper;
    private readonly IFileStorageService _fileStorageService;
    private readonly string containerName = "players";
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PlayersService(ApplicationDataContext context, IMapper mapper,
        IFileStorageService fileStorageService, IHttpContextAccessor httpContextAccessor)
    {
      _context = context;
      _mapper = mapper;
      _fileStorageService = fileStorageService;
      _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<PlayerDto>> Get([FromQuery] PaginationDto paginationDto)
    {
      var queryable = _context.Players.AsQueryable();
      await _httpContextAccessor.HttpContext.InsertParametersPaginationInHeader(queryable);
      var players = await queryable.OrderBy(x => x.Name).Paginate(paginationDto).ToListAsync();
      return _mapper.Map<List<PlayerDto>>(players);
    }

    public async Task<List<PlayersClubDto>> SearchByName([FromBody] string name)
    {
      if (string.IsNullOrWhiteSpace(name)) { return new List<PlayersClubDto>(); }
      return await _context.Players
          .Where(x => x.Name.Contains(name))
          .OrderBy(x => x.Name)
          .Select(x => new PlayersClubDto() { Id = x.Id, Name = x.Name, Picture = x.Picture })
          .Take(5)
          .ToListAsync();
    }

    public async Task<PlayerDto> Get(int id)
    {
      var player = await _context.Players.FirstOrDefaultAsync(x => x.Id == id);

      if (player == null)
      {
        throw new NotFoundException("Nie znaleziono elementu");
      }

      return _mapper.Map<PlayerDto>(player);
    }

    public async Task Post([FromForm] PlayerCreationDto playerCreationDto)
    {
      var player = _mapper.Map<Player>(playerCreationDto);

      if (playerCreationDto.Picture != null)
      {
        player.Picture = await _fileStorageService.SaveFile(containerName, playerCreationDto.Picture);
      }

      _context.Add(player);
      await _context.SaveChangesAsync();
    }

    public async Task Put(int id, [FromForm] PlayerCreationDto playerCreationDto)
    {
      var player = await _context.Players.FirstOrDefaultAsync(x => x.Id == id);

      if (player == null)
      {
        throw new NotFoundException("Nie znaleziono elementu");
      }

      player = _mapper.Map(playerCreationDto, player);

      if (playerCreationDto.Picture != null)
      {
        player.Picture = await _fileStorageService.EditFile(containerName,
            playerCreationDto.Picture, player.Picture);
      }

      await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
      var player = await _context.Players.FirstOrDefaultAsync(x => x.Id == id);

      if (player == null)
      {
        throw new NotFoundException("Nie znaleziono elementu");
      }

      _context.Remove(player);
      await _context.SaveChangesAsync();

      await _fileStorageService.DeleteFile(player.Picture, containerName);
    }

    public string SaveToCsv(IEnumerable<PlayerDto> components)
    {
      var headers = "Id;Name;DateOfBirth;Biography;Picture;";

      var csv = new StringBuilder(headers);

      csv.Append(Environment.NewLine);

      foreach (var component in components)
      {
        csv.Append(component.GetExportObject());
        csv.Append(Environment.NewLine);
      }
      csv.Append($"Count: {components.Count()}");
      csv.Append(Environment.NewLine);

      return csv.ToString();
    }
  }
}