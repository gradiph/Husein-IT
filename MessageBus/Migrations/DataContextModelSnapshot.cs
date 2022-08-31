﻿// <auto-generated />
using System;
using MessageBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MessageBus.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("ChannelSubscriber", b =>
                {
                    b.Property<int>("ChannelsId")
                        .HasColumnType("int");

                    b.Property<int>("SubscribersId")
                        .HasColumnType("int");

                    b.HasKey("ChannelsId", "SubscribersId");

                    b.HasIndex("SubscribersId");

                    b.ToTable("ChannelSubscriber");
                });

            modelBuilder.Entity("MessageBus.Models.Channel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Channels");
                });

            modelBuilder.Entity("MessageBus.Models.Message", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("ChannelId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("PublisherName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("ChannelId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("MessageBus.Models.MessageSubscriber", b =>
                {
                    b.Property<int>("MessageId")
                        .HasColumnType("int")
                        .HasColumnOrder(0);

                    b.Property<int>("SubscriberId")
                        .HasColumnType("int")
                        .HasColumnOrder(1);

                    b.Property<DateTime?>("SentAt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("MessageId", "SubscriberId");

                    b.HasIndex("SubscriberId");

                    b.ToTable("MessageSubscribers");
                });

            modelBuilder.Entity("MessageBus.Models.Subscriber", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Subscribers");
                });

            modelBuilder.Entity("ChannelSubscriber", b =>
                {
                    b.HasOne("MessageBus.Models.Channel", null)
                        .WithMany()
                        .HasForeignKey("ChannelsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MessageBus.Models.Subscriber", null)
                        .WithMany()
                        .HasForeignKey("SubscribersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MessageBus.Models.Message", b =>
                {
                    b.HasOne("MessageBus.Models.Channel", "Channel")
                        .WithMany("Messages")
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Channel");
                });

            modelBuilder.Entity("MessageBus.Models.MessageSubscriber", b =>
                {
                    b.HasOne("MessageBus.Models.Message", "Message")
                        .WithMany("Subscribers")
                        .HasForeignKey("MessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MessageBus.Models.Subscriber", "Subscriber")
                        .WithMany("Messages")
                        .HasForeignKey("SubscriberId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Message");

                    b.Navigation("Subscriber");
                });

            modelBuilder.Entity("MessageBus.Models.Channel", b =>
                {
                    b.Navigation("Messages");
                });

            modelBuilder.Entity("MessageBus.Models.Message", b =>
                {
                    b.Navigation("Subscribers");
                });

            modelBuilder.Entity("MessageBus.Models.Subscriber", b =>
                {
                    b.Navigation("Messages");
                });
#pragma warning restore 612, 618
        }
    }
}
