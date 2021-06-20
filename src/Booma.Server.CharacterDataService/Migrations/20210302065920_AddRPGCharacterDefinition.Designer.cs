﻿// <auto-generated />
using System;
using Glader.ASP.RPG;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Booma.Server.CharacterDataService.Migrations
{
    [DbContext(typeof(RPGCharacterDatabaseContext<PsobbCustomizationSlots, Vector3<ushort>, PsobbProportionSlots, Vector2<float>, CharacterRace, CharacterClass, DefaultTestSkillType, CharacterStatType>))]
    [Migration("20210302065920_AddRPGCharacterDefinition")]
    partial class AddRPGCharacterDefinition
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Glader.ASP.RPG.DBRPGCharacter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreationDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("LastModifiedDate")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("character");
                });

            modelBuilder.Entity("Glader.ASP.RPG.DBRPGCharacterCustomizableSlot<Booma.PsobbCustomizationSlots, Booma.Vector3<ushort>>", b =>
                {
                    b.Property<int>("CharacterId")
                        .HasColumnType("int");

                    b.Property<int>("SlotType")
                        .HasColumnType("int");

                    b.Property<int>("CustomizationId")
                        .HasColumnType("int");

                    b.HasKey("CharacterId", "SlotType");

                    b.HasIndex("CharacterId");

                    b.HasIndex("SlotType");

                    b.ToTable("character_customization_slot");
                });

            modelBuilder.Entity("Glader.ASP.RPG.DBRPGCharacterCustomizableSlotType<Booma.PsobbCustomizationSlots>", b =>
                {
                    b.Property<int>("SlotType")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("VisualName")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("SlotType");

                    b.ToTable("character_customization_slot_type");

                    b.HasData(
                        new
                        {
                            SlotType = 1,
                            Description = "",
                            VisualName = "Costume"
                        },
                        new
                        {
                            SlotType = 2,
                            Description = "",
                            VisualName = "Skin"
                        },
                        new
                        {
                            SlotType = 3,
                            Description = "",
                            VisualName = "Face"
                        },
                        new
                        {
                            SlotType = 4,
                            Description = "",
                            VisualName = "Head"
                        },
                        new
                        {
                            SlotType = 5,
                            Description = "",
                            VisualName = "Hair"
                        },
                        new
                        {
                            SlotType = 6,
                            Description = "",
                            VisualName = "Override"
                        });
                });

            modelBuilder.Entity("Glader.ASP.RPG.DBRPGCharacterDefinition<Booma.CharacterRace, Booma.CharacterClass>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<byte>("ClassId")
                        .HasColumnName("Class")
                        .HasColumnType("tinyint unsigned");

                    b.Property<int>("RaceId")
                        .HasColumnName("Race")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ClassId");

                    b.HasIndex("RaceId");

                    b.ToTable("character_definition");
                });

            modelBuilder.Entity("Glader.ASP.RPG.DBRPGCharacterOwnership", b =>
                {
                    b.Property<int>("OwnershipId")
                        .HasColumnType("int");

                    b.Property<int>("CharacterId")
                        .HasColumnType("int");

                    b.HasKey("OwnershipId", "CharacterId");

                    b.HasIndex("CharacterId");

                    b.HasIndex("OwnershipId");

                    b.ToTable("character_ownership");
                });

            modelBuilder.Entity("Glader.ASP.RPG.DBRPGCharacterProgress", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("Experience")
                        .HasColumnType("int");

                    b.Property<DateTime>("LastModifiedDate")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime(6)");

                    b.Property<int>("Level")
                        .HasColumnType("int");

                    b.Property<TimeSpan>("PlayTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("time(6)")
                        .HasDefaultValue(new TimeSpan(0, 0, 0, 0, 0));

                    b.HasKey("Id");

                    b.ToTable("character_progress");
                });

            modelBuilder.Entity("Glader.ASP.RPG.DBRPGCharacterProportionSlot<Booma.PsobbProportionSlots, Booma.Vector2<float>>", b =>
                {
                    b.Property<int>("CharacterId")
                        .HasColumnType("int");

                    b.Property<int>("SlotType")
                        .HasColumnType("int");

                    b.HasKey("CharacterId", "SlotType");

                    b.HasIndex("CharacterId");

                    b.HasIndex("SlotType");

                    b.ToTable("character_proportion_slot");
                });

            modelBuilder.Entity("Glader.ASP.RPG.DBRPGCharacterProportionSlotType<Booma.PsobbProportionSlots>", b =>
                {
                    b.Property<int>("SlotType")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("VisualName")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("SlotType");

                    b.ToTable("character_proportion_slot_type");

                    b.HasData(
                        new
                        {
                            SlotType = 1,
                            Description = "",
                            VisualName = "Default"
                        });
                });

            modelBuilder.Entity("Glader.ASP.RPG.DBRPGCharacterSkillKnown<Booma.DefaultTestSkillType>", b =>
                {
                    b.Property<int>("CharacterId")
                        .HasColumnType("int");

                    b.Property<int>("SkillId")
                        .HasColumnName("Skill")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreationDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime(6)");

                    b.HasKey("CharacterId", "SkillId");

                    b.HasIndex("CharacterId");

                    b.HasIndex("SkillId");

                    b.ToTable("character_skill_known");
                });

            modelBuilder.Entity("Glader.ASP.RPG.DBRPGCharacterSkillLevel<Booma.DefaultTestSkillType>", b =>
                {
                    b.Property<int>("CharacterId")
                        .HasColumnType("int");

                    b.Property<int>("SkillId")
                        .HasColumnName("Skill")
                        .HasColumnType("int");

                    b.Property<int>("Experience")
                        .HasColumnType("int");

                    b.Property<DateTime>("LastModifiedDate")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime(6)");

                    b.Property<int>("Level")
                        .HasColumnType("int");

                    b.HasKey("CharacterId", "SkillId");

                    b.HasIndex("CharacterId");

                    b.HasIndex("SkillId");

                    b.ToTable("character_skill_level");
                });

            modelBuilder.Entity("Glader.ASP.RPG.DBRPGClass<Booma.CharacterClass>", b =>
                {
                    b.Property<byte>("Id")
                        .HasColumnType("tinyint unsigned");

                    b.Property<string>("Description")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("VisualName")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("class");

                    b.HasData(
                        new
                        {
                            Id = (byte)0,
                            Description = "",
                            VisualName = "HUmar"
                        },
                        new
                        {
                            Id = (byte)1,
                            Description = "",
                            VisualName = "HUnewearl"
                        },
                        new
                        {
                            Id = (byte)2,
                            Description = "",
                            VisualName = "HUcast"
                        },
                        new
                        {
                            Id = (byte)3,
                            Description = "",
                            VisualName = "RAmar"
                        },
                        new
                        {
                            Id = (byte)4,
                            Description = "",
                            VisualName = "RAcast"
                        },
                        new
                        {
                            Id = (byte)5,
                            Description = "",
                            VisualName = "RAcaseal"
                        },
                        new
                        {
                            Id = (byte)6,
                            Description = "",
                            VisualName = "FOmarl"
                        },
                        new
                        {
                            Id = (byte)7,
                            Description = "",
                            VisualName = "FOnewm"
                        },
                        new
                        {
                            Id = (byte)8,
                            Description = "",
                            VisualName = "FOnewearl"
                        },
                        new
                        {
                            Id = (byte)9,
                            Description = "",
                            VisualName = "HUcaseal"
                        },
                        new
                        {
                            Id = (byte)10,
                            Description = "",
                            VisualName = "FOmar"
                        },
                        new
                        {
                            Id = (byte)11,
                            Description = "",
                            VisualName = "RAmarl"
                        });
                });

            modelBuilder.Entity("Glader.ASP.RPG.DBRPGRace<Booma.CharacterRace>", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("VisualName")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("race");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Description = "",
                            VisualName = "Human"
                        },
                        new
                        {
                            Id = 2,
                            Description = "",
                            VisualName = "Newman"
                        },
                        new
                        {
                            Id = 3,
                            Description = "",
                            VisualName = "Cast"
                        });
                });

            modelBuilder.Entity("Glader.ASP.RPG.DBRPGSkill<Booma.DefaultTestSkillType>", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("IsPassiveSkill")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("VisualName")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("skill");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Description = "",
                            IsPassiveSkill = false,
                            VisualName = "Default"
                        });
                });

            modelBuilder.Entity("Glader.ASP.RPG.DBRPGCharacterCustomizableSlot<Booma.PsobbCustomizationSlots, Booma.Vector3<ushort>>", b =>
                {
                    b.HasOne("Glader.ASP.RPG.DBRPGCharacter", null)
                        .WithMany()
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Glader.ASP.RPG.DBRPGCharacterCustomizableSlotType<Booma.PsobbCustomizationSlots>", "SlotDefinition")
                        .WithMany()
                        .HasForeignKey("SlotType")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("Booma.Vector3<ushort>", "SlotColor", b1 =>
                        {
                            b1.Property<int>("DBRPGCharacterCustomizableSlot<PsobbCustomizationSlots, Vector3<ushort>>CharacterId")
                                .HasColumnType("int");

                            b1.Property<int>("DBRPGCharacterCustomizableSlot<PsobbCustomizationSlots, Vector3<ushort>>SlotType")
                                .HasColumnType("int");

                            b1.Property<ushort>("X")
                                .HasColumnType("smallint unsigned");

                            b1.Property<ushort>("Y")
                                .HasColumnType("smallint unsigned");

                            b1.Property<ushort>("Z")
                                .HasColumnType("smallint unsigned");

                            b1.HasKey("DBRPGCharacterCustomizableSlot<PsobbCustomizationSlots, Vector3<ushort>>CharacterId", "DBRPGCharacterCustomizableSlot<PsobbCustomizationSlots, Vector3<ushort>>SlotType");

                            b1.ToTable("character_customization_slot");

                            b1.WithOwner()
                                .HasForeignKey("DBRPGCharacterCustomizableSlot<PsobbCustomizationSlots, Vector3<ushort>>CharacterId", "DBRPGCharacterCustomizableSlot<PsobbCustomizationSlots, Vector3<ushort>>SlotType");
                        });
                });

            modelBuilder.Entity("Glader.ASP.RPG.DBRPGCharacterDefinition<Booma.CharacterRace, Booma.CharacterClass>", b =>
                {
                    b.HasOne("Glader.ASP.RPG.DBRPGClass<Booma.CharacterClass>", "Class")
                        .WithMany()
                        .HasForeignKey("ClassId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Glader.ASP.RPG.DBRPGCharacter", "Character")
                        .WithMany()
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Glader.ASP.RPG.DBRPGRace<Booma.CharacterRace>", "Race")
                        .WithMany()
                        .HasForeignKey("RaceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Glader.ASP.RPG.DBRPGCharacterOwnership", b =>
                {
                    b.HasOne("Glader.ASP.RPG.DBRPGCharacter", "Character")
                        .WithMany()
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Glader.ASP.RPG.DBRPGCharacterProgress", b =>
                {
                    b.HasOne("Glader.ASP.RPG.DBRPGCharacter", "Character")
                        .WithOne("Progress")
                        .HasForeignKey("Glader.ASP.RPG.DBRPGCharacterProgress", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Glader.ASP.RPG.DBRPGCharacterProportionSlot<Booma.PsobbProportionSlots, Booma.Vector2<float>>", b =>
                {
                    b.HasOne("Glader.ASP.RPG.DBRPGCharacter", null)
                        .WithMany()
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Glader.ASP.RPG.DBRPGCharacterProportionSlotType<Booma.PsobbProportionSlots>", "SlotDefinition")
                        .WithMany()
                        .HasForeignKey("SlotType")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("Booma.Vector2<float>", "Proportion", b1 =>
                        {
                            b1.Property<int>("DBRPGCharacterProportionSlot<PsobbProportionSlots, Vector2<float>>CharacterId")
                                .HasColumnType("int");

                            b1.Property<int>("DBRPGCharacterProportionSlot<PsobbProportionSlots, Vector2<float>>SlotType")
                                .HasColumnType("int");

                            b1.Property<float>("X")
                                .HasColumnType("float");

                            b1.Property<float>("Y")
                                .HasColumnType("float");

                            b1.HasKey("DBRPGCharacterProportionSlot<PsobbProportionSlots, Vector2<float>>CharacterId", "DBRPGCharacterProportionSlot<PsobbProportionSlots, Vector2<float>>SlotType");

                            b1.ToTable("character_proportion_slot");

                            b1.WithOwner()
                                .HasForeignKey("DBRPGCharacterProportionSlot<PsobbProportionSlots, Vector2<float>>CharacterId", "DBRPGCharacterProportionSlot<PsobbProportionSlots, Vector2<float>>SlotType");
                        });
                });

            modelBuilder.Entity("Glader.ASP.RPG.DBRPGCharacterSkillKnown<Booma.DefaultTestSkillType>", b =>
                {
                    b.HasOne("Glader.ASP.RPG.DBRPGCharacter", null)
                        .WithMany()
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Glader.ASP.RPG.DBRPGSkill<Booma.DefaultTestSkillType>", "Skill")
                        .WithMany()
                        .HasForeignKey("SkillId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Glader.ASP.RPG.DBRPGCharacterSkillLevel<Booma.DefaultTestSkillType>", b =>
                {
                    b.HasOne("Glader.ASP.RPG.DBRPGCharacter", null)
                        .WithMany()
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Glader.ASP.RPG.DBRPGSkill<Booma.DefaultTestSkillType>", "Skill")
                        .WithMany()
                        .HasForeignKey("SkillId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Glader.ASP.RPG.DBRPGCharacterSkillKnown<Booma.DefaultTestSkillType>", "KnownSkill")
                        .WithOne("SkillLevelData")
                        .HasForeignKey("Glader.ASP.RPG.DBRPGCharacterSkillLevel<Booma.DefaultTestSkillType>", "CharacterId", "SkillId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
